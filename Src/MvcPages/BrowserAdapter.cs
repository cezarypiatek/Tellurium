using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Web.Mvc;
using OpenQA.Selenium;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Remote;
using Tellurium.MvcPages.BrowserCamera;
using Tellurium.MvcPages.EndpointCoverage;
using Tellurium.MvcPages.SeleniumUtils;
using Tellurium.MvcPages.SeleniumUtils.Exceptions;
using Tellurium.MvcPages.WebPages;
using Tellurium.MvcPages.WebPages.WebForms;

namespace Tellurium.MvcPages
{
    public class BrowserAdapter : IBrowserAdapter
    {
        private RemoteWebDriver Driver { get; set; }
        private IBrowserCamera browserCamera;
        private INavigator navigator;
        private List<IFormInputAdapter> supportedInputsAdapters;
        private string BrowserName { get; set; }
        private int NumberOfInputSetRetries { get; set; }
        private AfterFieldValueSet AfterFieldValueSetAction { get; set; }
        private IReadOnlyCollection<string> availableEndpoints = new List<string>();
        public static IBrowserAdapter Create(BrowserAdapterConfig config)
        {
            var browserAdapter = new BrowserAdapter();
            browserAdapter.Driver = SeleniumDriverFactory.CreateLocalDriver(config.BrowserType, config.SeleniumDriversPath);
            var browserCameraConfig = config.BrowserCameraConfig ?? BrowserCameraConfig.CreateDefault();
            browserAdapter.browserCamera = BrowserCamera.BrowserCamera.CreateNew(browserAdapter.Driver, browserCameraConfig);
            browserAdapter.navigator = new Navigator(browserAdapter.Driver, config.PageUrl, config.MeasureEndpointCoverage);
            browserAdapter.supportedInputsAdapters = config.InputAdapters.ToList();
            browserAdapter.SetupBrowserDimensions(config.BrowserDimensions);
            browserAdapter.BrowserName = config.BrowserType.ToString();
            browserAdapter.NumberOfInputSetRetries = config.NumberOfInputSetRetries;
            browserAdapter.AfterFieldValueSetAction = config.AfterFieldValueSetAction;
            browserAdapter.availableEndpoints = config.GetAvailableEndpoints().ToList();
            if (config.AnimationsDisabled)
            {
                browserAdapter.navigator.PageReload += (sender, args) => browserAdapter.Driver.DisableAnimations();
            }
            return browserAdapter;
        }

        public static void Execute(BrowserAdapterConfig config, Action<IBrowserAdapter> action)
        {
            using (var browserAdapter = Create(config))
            {
                try
                {
                    action(browserAdapter);
                }
                catch (Exception)
                {
                    SaveErrorScreenshot(browserAdapter, config);
                    throw;
                }
            }
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

        public void NavigateTo(string subpagePath)
        {
            navigator.NavigateTo(subpagePath);
        }

        public byte [] TakeScreenshot()
        {
            return browserCamera.TakeScreenshot();
        }

        public void SaveScreenshot(string directoryPath, string screenshotName, bool addBrowserPrefix=true)
        {
            var screenshotRawData = browserCamera.TakeScreenshot();
            var fullScreenshotName = addBrowserPrefix ? $"{BrowserName}_{screenshotName}"
                : screenshotName;
            var screenshotStorage = new FileSystemScreenshotStorage(directoryPath);
            screenshotStorage.Persist(screenshotRawData, fullScreenshotName);
        }

        private static void SaveErrorScreenshot(IBrowserAdapter browserAdapter, BrowserAdapterConfig config)
        {
            string screenshotName = $"Error{DateTime.Now:yyyy_MM_dd__HH_mm_ss}";
            browserAdapter.SaveScreenshot(config.ScreenshotsPath, screenshotName);
        }

        public  MvcWebForm<TModel> GetForm<TModel>(string formId)
        {
            var formElement = this.Driver.GetElementById(formId);
            return new MvcWebForm<TModel>(formElement, Driver, supportedInputsAdapters, this.NumberOfInputSetRetries, this.AfterFieldValueSetAction);
        }

        public WebForm GetForm(string formId)
        {
            var formElement = this.Driver.GetElementById(formId);
            return new WebForm(formElement, Driver, supportedInputsAdapters, this.NumberOfInputSetRetries, this.AfterFieldValueSetAction);
        }

        public  void ClickOn(string elementId)
        {
            var elementToClick = this.Driver.GetElementById(elementId);
            Driver.ClickOn(elementToClick);
        }

        public void HoverOn(string elementId)
        {
            var elementToHover = this.Driver.GetElementById(elementId);
            Driver.HoverOn(elementToHover);
        }

        public IPageFragment GetPageFragmentById(string elementId)
        {
            var pageFragment = Driver.GetElementById(elementId);
            return new PageFragment(Driver, pageFragment);
        }

        public void RefreshPage()
        {
            this.navigator.RefreshPage();
        }
        
        public void WaitForElementWithId(string elementId, int timeOut = 30)
        {
            Driver.GetElementById(elementId, timeOut);
        }

        public void Wait(int seconds)
        {
            Thread.Sleep(seconds * 1000);
        }

        public PageFragmentWatcher WatchForContentChange(string containerId)
        {
            return Driver.WatchForContentChanges(containerId);
        }

        public void AffectElementWith(string elementId, Action action)
        {
            var watcher = WatchForContentChange(elementId);
            action();
            watcher.WaitForChange();
        }

        public void ReloadPageWith(Action action)
        {
            Driver.ExecuteScript("window.__selenium_visited__ = true;");
            navigator.OnBeforePageReload();
            action();
            try
            {
                Driver.WaitUntil(SeleniumExtensions.PageLoadTimeout,
                    driver =>
                        Driver.IsPageLoaded() &&
                        (bool) Driver.ExecuteScript("return  window.__selenium_visited__ === undefined;"));
                navigator.OnPageReload();
            }
            catch (WebDriverTimeoutException)
            {
                throw new CannotReloadPageWithException();
            }
        }

        public void DisableAnimations()
        {
           this.Driver.DisableAnimations();
        }

        public void EnableAnimations()
        {
            this.Driver.EnableAnimations();
        }

        public void Dispose()
        {
            var visitedEndpoints = this.navigator.GetAllRequestedEndpoints();
            EndpointCoverageReportGenerator.GenerateEndpointCoverageReport(this.availableEndpoints, visitedEndpoints);
            Driver.Close();
            Driver.Quit();
        }

        public void ClickOnElementWithText(string text)
        {
            Driver.ClickOnElementWithText(Driver, text, false);
        }

        public void ClickOnElementWithPartialText(string text)
        {
            Driver.ClickOnElementWithText(Driver, text, true);
        }

        public void HoverOnElementWithText(string text)
        {
            Driver.HoverOnElementWithText(Driver, text, false);
        }

        public void HoverOnElementWithPartialText(string text)
        {
            Driver.HoverOnElementWithText(Driver, text, true);
        }

        public WebList GetListWithId(string id)
        {
            return Driver.GetListWithId(id);
        }

        public WebList ToWebList()
        {
            var body = Driver.FindElementByTagName("body");
            var mainPageFragment = new PageFragment(Driver, body);
            return mainPageFragment.ToWebList();
        }

        public IWebDriver WrappedDriver => Driver;
    }

    public interface IBrowserAdapter : IPageFragment, IBrowserCamera,  IDisposable, IWrapsDriver
    {
        /// <summary>
        /// Return strongly typed adapter for web form with given id
        /// </summary>
        /// <typeparam name="TModel">Model connected with form</typeparam>
        /// <param name="formId">Id of expected form</param>
        MvcWebForm<TModel> GetForm<TModel>(string formId);


        /// <summary>
        /// Return weakly typed adapter for web form with given id
        /// </summary>
        /// <param name="formId">Id of expected form</param>
        WebForm GetForm(string formId);

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
        /// Simulate hover event on element with given id
        /// </summary>
        /// <param name="elementId">Id of expected element</param>
        void HoverOn(string elementId);

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


        /// <summary>
        /// Navigate to page represented by given controller's action
        /// </summary>
        /// <param name="action">Expression to given action></param>
        void NavigateTo<TController>(Expression<Action<TController>> action) where TController : Controller;

        /// <summary>
        /// Navigate to page related to root page
        /// </summary>
        /// <param name="subpagePath">Path to page</param>
        void NavigateTo(string subpagePath);

        void SaveScreenshot(string directoryPath, string screenshotName, bool addBrowserPrefix=true);

        /// <summary>
        /// Wait explicitly given amount of seconds
        /// </summary>
        /// <param name="seconds"></param>
        void Wait(int seconds);

        /// <summary>
        /// Start obserwing container with given id for contnet change
        /// </summary>
        /// <param name="containerId">Container id</param>
        PageFragmentWatcher WatchForContentChange(string containerId);

        /// <summary>
        /// Perform action and wait until element with given id will change.
        /// </summary>
        /// <param name="elementId">Id of observed element</param>
        /// <param name="action">Action that should have impact on observed element</param>
        void AffectElementWith(string elementId, Action action);

        /// <summary>
        /// Perform action and wait until page will reaload
        /// </summary>
        /// <param name="action">Action that should cause page reload</param>
        void ReloadPageWith(Action action);

        /// <summary>
        /// Disable animations on page
        /// </summary>
        void DisableAnimations();

        /// <summary>
        /// Restore animations on page
        /// </summary>
        void EnableAnimations();
       
    }
}