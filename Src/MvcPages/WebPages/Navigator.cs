using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using OpenQA.Selenium.Remote;
using Tellurium.MvcPages.EndpointCoverage;

namespace Tellurium.MvcPages.WebPages
{
    public interface INavigator
    {
        void NavigateTo<TController>(Expression<Action<TController>> action);
        void NavigateTo(string subpagePath);
        void OnPageReload();
        void RefreshPage();
        event EventHandler<PageReloadEventArgs> PageReload;
        void OnBeforePageReload();
    }

    public class Navigator : INavigator, IEndpointCollector
    {
        private readonly RemoteWebDriver driver;
        private readonly string rootUrl;
        private readonly bool measureCoverage;
        private static readonly Regex UrlPattern = new Regex("^https?://");
        private readonly  ISet<string> requestedEndpoints = new HashSet<string>();

        public Navigator(RemoteWebDriver driver, string rootUrl, bool measureCoverage)
        {
            if (UrlPattern.IsMatch(rootUrl) == false)
            {
                throw new ArgumentException("Invalid rootUrl. It should start with http:// or https://");
            }
            this.driver = driver;
            this.rootUrl = rootUrl;
            this.measureCoverage = measureCoverage;
        }

        public void NavigateTo<TController>(Expression<Action<TController>> action)
        {
            var actionAddress = UrlHelper.BuildActionAddressFromExpression(action);
            NavigateTo(actionAddress);
        }

        public void NavigateTo(string subpagePath)
        {
            OnBeforePageReload();
            var url = $"{rootUrl}/{subpagePath}";
            driver.Navigate().GoToUrl(url);
            OnPageReload();
        }

        public void RefreshPage()
        {
            OnBeforePageReload();
            this.driver.Navigate().Refresh();
            OnPageReload();
        }

        public event EventHandler<PageReloadEventArgs> PageReload;

        public void OnBeforePageReload()
        {
            CollectRequestedEndpointsData();
        }

        public void OnPageReload()
        {
            var handler = PageReload;
            if (handler != null)
            {
                handler.Invoke(this, new PageReloadEventArgs()
                {
                    NewUrl = driver.Url
                });
            }
        }

        private void CollectRequestedEndpointsData()
        {
            if (measureCoverage == false)
            {
                return;
            }
            var requestedEndpointsOnPage = (IReadOnlyCollection<object>) this.driver.ExecuteScript("return window.performance ? performance.getEntries().map(function(el){return el.name}):[]");
            foreach (var endpoint in requestedEndpointsOnPage)
            {
                this.requestedEndpoints.Add((string)endpoint);
            }

            this.requestedEndpoints.Add(this.driver.Url);
        }

        public IReadOnlyCollection<string> GetAllRequestedEndpoints()
        {
            CollectRequestedEndpointsData();
            return this.requestedEndpoints.Where(x => x.StartsWith("http")).Select(x => MvcEndpointsHelper.NormalizeEndpointAddress(new Uri(x).LocalPath)).ToArray();
        }
    }
       
    public class PageReloadEventArgs:EventArgs
    {
        public string NewUrl { get; set; }
    }
}