using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using MaintainableSelenium.MvcPages.BrowserCamera;
using MaintainableSelenium.MvcPages.SeleniumUtils;
using MaintainableSelenium.MvcPages.WebPages;
using MaintainableSelenium.MvcPages.WebPages.WebForms;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;

namespace MaintainableSelenium.MvcPages
{
    public class BrowserAdapter : IBrowserAdapter
    {
        private RemoteWebDriver Driver { get; set; }
        private IBrowserCamera browserCamera;
        private INavigator navigator;
        private List<IFormInputAdapter> supportedInputsAdapters;

        public static IBrowserAdapter Create(BrowserAdapterConfig config)
        {
            var browserAdapter = new BrowserAdapter();
            browserAdapter.Driver = SeleniumDriverFactory.CreateLocalDriver(config.BrowserType, config.SeleniumDriversPath);
            browserAdapter.browserCamera = BrowserCamera.BrowserCamera.CreateNew(browserAdapter.Driver, config.BrowserType.ToString(), config.BrowserCameraConfig);
            browserAdapter.navigator = new Navigator(browserAdapter.Driver, config.PageUrl);
            browserAdapter.supportedInputsAdapters = config.InputAdapters.ToList();
            browserAdapter.SetupBrowserDimensions(config.BrowserDimensions);
            return browserAdapter;
        }

        public  void SetupBrowserDimensions(BrowserDimensionsConfig dimensionsConfig)
        {
            var browserOptions = this.Driver.Manage();
            if (dimensionsConfig == null)
            {
                browserOptions.Window.Maximize();
            }
            else
            {
                browserOptions.Window.Size = new Size
                {
                    Width = dimensionsConfig.Width,
                    Height = dimensionsConfig.Height
                };
            }
        }


        public void TakeScreenshot(string name)
        {
            browserCamera.TakeScreenshot(name);
        }

        public void NavigateTo<TController>(Expression<Action<TController>> action) where TController : Controller
        {
            navigator.NavigateTo(action);
        }

        public  WebForm<TModel> GetForm<TModel>(string formId)
        {
            var formElement = this.Driver.GetElementById(formId);
            return new WebForm<TModel>(formElement, Driver, supportedInputsAdapters);
        }

        public  void ClickOn(string elementId)
        {
            this.Driver.GetElementById(elementId).Click();
        }

        public IPageFragment GetPageFragmentById(string elementId)
        {
            var pageFragment = Driver.GetElementById(elementId);
            return new PageFragment(Driver, pageFragment);
        }

        public void RefreshPage()
        {
            this.Driver.Navigate().Refresh();
        }
        
        public void WaitForElementWithId(string elementId, int timeOut = 30)
        {
            var waiter = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeOut));
            waiter.Until(ExpectedConditions.ElementIsVisible(By.Id(elementId)));
        }

        public void Dispose()
        {
            Driver.Close();
            Driver.Quit();
        }

        public void ClickOnElementWithText(string linkText)
        {
            Driver.ClickOnElementWithText(Driver.FindElementByTagName("body"), linkText);
        }
    }

    public interface IBrowserAdapter : IPageFragment,  IDisposable
    {
        /// <summary>
        /// Return strongly typed adapter for web form with given id
        /// </summary>
        /// <typeparam name="TModel">Model connected with form</typeparam>
        /// <param name="formId">Id of expected form</param>
        WebForm<TModel> GetForm<TModel>(string formId);

        /// <summary>
        /// Refresh page
        /// </summary>
        void RefreshPage();

        /// <summary>
        /// Simulate click event on element with given id
        /// </summary>
        /// <param name="elementId">Id of expected element</param>
        void ClickOn(string elementId);

        /// <summary>
        /// Return page fragment with given id
        /// </summary>
        /// <param name="elementId">Id of expected element</param>
        IPageFragment GetPageFragmentById(string elementId);

        /// <summary>
        /// Stop execution until element with given id appear
        /// </summary>
        /// <param name="elementId">Id of expected element</param>
        /// <param name="timeOut">Max time in seconds to wait</param>
        void WaitForElementWithId(string elementId, int timeOut = 30);

        void TakeScreenshot(string name);

        void NavigateTo<TController>(Expression<Action<TController>> action) where TController : Controller;
    }
}