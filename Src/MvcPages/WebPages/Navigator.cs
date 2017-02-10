using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using OpenQA.Selenium.Remote;

namespace Tellurium.MvcPages.WebPages
{
    public interface INavigator
    {
        void NavigateTo<TController>(Expression<Action<TController>> action) where TController : Controller;
        void NavigateTo(string subpagePath);
        void OnPageReload();
        void RefreshPage();
        event EventHandler<PageReloadEventArgs> PageReload;
        IReadOnlyCollection<string> GetAllRequestedEndpoints();
    }

    public class Navigator : INavigator
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

        public void NavigateTo<TController>(Expression<Action<TController>> action) where TController : Controller
        {
            var actionAddress = UrlHelper.BuildActionAddressFromExpression(action);
            NavigateTo(actionAddress);
        }

        public void NavigateTo(string subpagePath)
        {
            CollectRequestedEndpointsData();
            var url = $"{rootUrl}/{subpagePath}";
            driver.Navigate().GoToUrl(url);
            OnPageReload();
        }

        public void RefreshPage()
        {
            CollectRequestedEndpointsData();
            this.driver.Navigate().Refresh();
            OnPageReload();
        }

        public event EventHandler<PageReloadEventArgs> PageReload;

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
            return this.requestedEndpoints.Select(x=> new Uri(x).LocalPath.TrimEnd('/')).ToArray();
        }
    }
       
    public class PageReloadEventArgs:EventArgs
    {
        public string NewUrl { get; set; }
    }
}