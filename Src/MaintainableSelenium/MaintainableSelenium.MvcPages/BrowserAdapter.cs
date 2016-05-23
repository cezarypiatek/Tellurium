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
using OpenQA.Selenium.Remote;

namespace MaintainableSelenium.MvcPages
{
    public class BrowserAdapter : IBrowserAdapter
    {
        private RemoteWebDriver Driver { get; set; }
        private IBrowserCamera browserCamera;
        private INavigator navigator;
        private List<IFormInputAdapter> supportedInputsAdapters;
        private IScreenshotStorage screenshotStorage;
        private string BrowserName { get; set; }

        public static IBrowserAdapter Create(BrowserAdapterConfig config)
        {
            var browserAdapter = new BrowserAdapter();
            browserAdapter.Driver = SeleniumDriverFactory.CreateLocalDriver(config.BrowserType, config.SeleniumDriversPath);
            var browserCameraConfig = config.BrowserCameraConfig ?? BrowserCameraConfig.CreateDefault();
            browserAdapter.browserCamera = BrowserCamera.BrowserCamera.CreateNew(browserAdapter.Driver, browserCameraConfig);
            browserAdapter.navigator = new Navigator(browserAdapter.Driver, config.PageUrl);
            browserAdapter.screenshotStorage = new FileSystemScreenshotStorage(config.ScreenshotsPath);
            browserAdapter.supportedInputsAdapters = config.InputAdapters.ToList();
            browserAdapter.SetupBrowserDimensions(config.BrowserDimensions);
            browserAdapter.BrowserName = config.BrowserType.ToString();
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

        public void NavigateTo<TController>(Expression<Action<TController>> action) where TController : Controller
        {
            navigator.NavigateTo(action);
        }

        public byte [] TakeScreenshot()
        {
            return browserCamera.TakeScreenshot();
        }

        public void SaveScreenshot(string screenshotName)
        {
            var screenshotRawData = browserCamera.TakeScreenshot();
            var fullScreenshotName = string.Format("{0}_{1}", BrowserName, screenshotName);
            this.screenshotStorage.Persist(screenshotRawData, fullScreenshotName);
        }

        public  WebForm<TModel> GetForm<TModel>(string formId)
        {
            var formElement = this.Driver.GetElementById(formId);
            return new WebForm<TModel>(formElement, Driver, supportedInputsAdapters);
        }

        public  void ClickOn(string elementId)
        {
            var elementToClick = this.Driver.GetElementById(elementId);
            Driver.ClickOn(elementToClick);
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
            Driver.GetElementById(elementId, timeOut);
        }

        public void Wait(int seconds)
        {
            Driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(seconds));
        }

        public void WaitForContentChange(string containerId, int timeOut = 30)
        {
            Driver.WaitForContentChange(containerId, timeOut);
        }


        public void Dispose()
        {
            Driver.Close();
            Driver.Quit();
        }

        public void ClickOnElementWithText(string linkText)
        {
            var scope = Driver.FindElementByTagName("body");
            Driver.ClickOnElementWithText(scope, linkText);
        }
    }

    public interface IBrowserAdapter : IPageFragment, IBrowserCamera,  IDisposable
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

        byte [] TakeScreenshot();

        void NavigateTo<TController>(Expression<Action<TController>> action) where TController : Controller;

        void SaveScreenshot(string screenshotName);
        void Wait(int seconds);
        void WaitForContentChange(string containerId, int timeOut = 30);
    }
}