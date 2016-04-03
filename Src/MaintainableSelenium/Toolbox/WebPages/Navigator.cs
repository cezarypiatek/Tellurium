using System;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using OpenQA.Selenium.Remote;

namespace MaintainableSelenium.Toolbox.WebPages
{
    public interface INavigator
    {
        void NavigateTo<TController>(Expression<Action<TController>> action) where TController : Controller;
    }

    public class Navigator : INavigator
    {
        private readonly RemoteWebDriver driver;
        private readonly string rootUrl;
        private static readonly Regex UrlPattern = new Regex("^https?://");
        public Navigator(RemoteWebDriver driver, string rootUrl)
        {
            if (UrlPattern.IsMatch(rootUrl) == false)
            {
                throw new ArgumentException("Invalid rootUrl. It should start with http:// or https://");
            }
            this.driver = driver;
            this.rootUrl = rootUrl;
        }

        public void NavigateTo<TController>(Expression<Action<TController>> action) where TController : Controller
        {
            var actionAddress = UrlHelper.BuildActionAddressFromExpression(action);
            var url = string.Format("{0}/{1}", rootUrl, actionAddress);
            driver.Navigate().GoToUrl(url);
        }
    }
}