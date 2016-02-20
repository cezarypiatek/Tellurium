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

namespace MaintainableSelenium.Toolbox.Drivers
{
    public static class SeleniumDriverFactory
    {
        public static RemoteWebDriver CreateLocalDriver(SeleniumDriverType driverType, string driversPath)
        {
            switch (driverType)
            {
                case SeleniumDriverType.Firefox:
                    return new FirefoxDriver();
                case SeleniumDriverType.Chrome:
                    return new ChromeDriver(driversPath);
                case SeleniumDriverType.InternetExplorer:
                    return new InternetExplorerDriver(driversPath);
                case SeleniumDriverType.Opera:
                    return new OperaDriver(driversPath);
                case SeleniumDriverType.Safari:
                    return new SafariDriver(new SafariOptions() { SafariLocation = driversPath });
                case SeleniumDriverType.Phantom:
                    return new PhantomJSDriver(driversPath);
                case SeleniumDriverType.Edge:
                    return new EdgeDriver(driversPath);
                default:
                    throw new ArgumentOutOfRangeException("driverType", driverType, null);
            }
        }

        public static RemoteWebDriver CreateRemoteDriver(SeleniumDriverType driverType, string seleniumServerUrl)
        {
            var browserCapabilities = GetBrowserCapabilities(driverType);
            return new RemoteWebDriver(new Uri(seleniumServerUrl), browserCapabilities);
        }

        private static ICapabilities GetBrowserCapabilities(SeleniumDriverType driverType)
        {
            switch (driverType)
            {
                case SeleniumDriverType.Firefox:
                    return DesiredCapabilities.Firefox();
                case SeleniumDriverType.Chrome:
                    return DesiredCapabilities.Chrome();
                case SeleniumDriverType.InternetExplorer:
                    return DesiredCapabilities.InternetExplorer();
                case SeleniumDriverType.Opera:
                    return DesiredCapabilities.Opera();
                case SeleniumDriverType.Safari:
                    return DesiredCapabilities.Safari();
                case SeleniumDriverType.Phantom:
                    return DesiredCapabilities.PhantomJS();
                case SeleniumDriverType.Edge:
                    return DesiredCapabilities.Edge();
                default:
                    throw new ArgumentOutOfRangeException("driverType", driverType, null);
            }
        }
    }

    public enum SeleniumDriverType
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
