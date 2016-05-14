using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Opera;
using OpenQA.Selenium.PhantomJS;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Safari;

namespace MaintainableSelenium.MvcPages.SeleniumUtils
{
    public static class SeleniumDriverFactory
    {
        public static RemoteWebDriver CreateLocalDriver(BrowserType driverType, string driversPath)
        {
            switch (driverType)
            {
                case BrowserType.Firefox:
                    var profile = new FirefoxProfile {DeleteAfterUse = true};
                    return new FirefoxDriver(profile);
                case BrowserType.Chrome:
                    return new ChromeDriver(driversPath);
                case BrowserType.InternetExplorer:
                    return new InternetExplorerDriver(driversPath);
                case BrowserType.Opera:
                    return new OperaDriver(driversPath);
                case BrowserType.Safari:
                    return new SafariDriver(new SafariOptions() { SafariLocation = driversPath });
                case BrowserType.Phantom:
                    return new PhantomJSDriver(driversPath);
                case BrowserType.Edge:
                    return new EdgeDriver(driversPath);
                default:
                    throw new ArgumentOutOfRangeException("driverType", driverType, null);
            }
        }

        public static RemoteWebDriver CreateRemoteDriver(BrowserType driverType, string seleniumServerUrl)
        {
            var browserCapabilities = GetBrowserCapabilities(driverType);
            return new RemoteWebDriver(new Uri(seleniumServerUrl), browserCapabilities);
        }

        private static ICapabilities GetBrowserCapabilities(BrowserType driverType)
        {
            switch (driverType)
            {
                case BrowserType.Firefox:
                    return DesiredCapabilities.Firefox();
                case BrowserType.Chrome:
                    return DesiredCapabilities.Chrome();
                case BrowserType.InternetExplorer:
                    return DesiredCapabilities.InternetExplorer();
                case BrowserType.Opera:
                    return DesiredCapabilities.Opera();
                case BrowserType.Safari:
                    return DesiredCapabilities.Safari();
                case BrowserType.Phantom:
                    return DesiredCapabilities.PhantomJS();
                case BrowserType.Edge:
                    return DesiredCapabilities.Edge();
                default:
                    throw new ArgumentOutOfRangeException("driverType", driverType, null);
            }
        }
    }

    public enum BrowserType
    {
        Firefox = 1,
        Chrome,
        InternetExplorer,
        Opera,
        Safari,
        Phantom,
        Edge
    }
}
