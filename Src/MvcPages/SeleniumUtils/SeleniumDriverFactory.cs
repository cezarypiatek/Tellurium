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
using Tellurium.MvcPages.Configuration;
using Tellurium.MvcPages.SeleniumUtils.WebDrivers;

namespace Tellurium.MvcPages.SeleniumUtils
{
    public static class SeleniumDriverFactory
    {
        public static RemoteWebDriver CreateDriver(BrowserAdapterConfig config)
        {
            if (config.UseRemoteDriver)
            {
                return CreateRemoteDriver(config.BrowserType, config.SeleniumServerUrl);
            }
            return CreateLocalDriver(config.BrowserType, config.SeleniumDriversPath);
        }

        private static readonly TimeSpan BrowserLoadTimeout = TimeSpan.FromSeconds(60.0);

        public static RemoteWebDriver CreateLocalDriver(BrowserType driverType, string driversPath)
        {
            switch (driverType)
            {
                case BrowserType.Firefox:
                    var firefoxOptions = new FirefoxOptions
                    {
                        Profile = new FirefoxProfile {DeleteAfterUse = true},
                        UseLegacyImplementation = true
                    };
                    var firefoxService = FirefoxDriverService.CreateDefaultService(driversPath);
                    return new FirefoxDriver(firefoxService, firefoxOptions, BrowserLoadTimeout);
                case BrowserType.FirefoxGecko:
                    var firefoxGeckoOptions = new FirefoxOptions
                    {
                        Profile = new FirefoxProfile {DeleteAfterUse = true}
                    };
                    var firefoxGeckoSerice = FirefoxDriverService.CreateDefaultService(driversPath);
                    return new FirefoxDriver(firefoxGeckoSerice, firefoxGeckoOptions, BrowserLoadTimeout);
                case BrowserType.Chrome:
                    return new ChromeDriver(driversPath);
                case BrowserType.ChromeExtended:
                    return new ChromeDriverExtended(driversPath);
                case BrowserType.InternetExplorer:
                    return new InternetExplorerDriver(driversPath);
                case BrowserType.Opera:
                    return new OperaDriver(driversPath);
                case BrowserType.Safari:
                    var safariService = SafariDriverService.CreateDefaultService(driversPath);
                    return new SafariDriver(safariService);
                case BrowserType.Phantom:
                    return new PhantomJSDriver(driversPath);
                case BrowserType.Edge:
                    return new EdgeDriver(driversPath);
                default:
                    throw new ArgumentOutOfRangeException(nameof(driverType), driverType, null);
            }
        }

        public static RemoteWebDriver CreateRemoteDriver(BrowserType driverType, string seleniumServerUrl)
        {
            if (string.IsNullOrWhiteSpace(seleniumServerUrl))
            {
                throw new ArgumentException("For remote driver selenium server url is required.");
            }
            var browserCapabilities = GetBrowserCapabilities(driverType);
            return new RemoteWebDriver(new Uri(seleniumServerUrl), browserCapabilities);
        }

        private static ICapabilities GetBrowserCapabilities(BrowserType driverType)
        {
            switch (driverType)
            {
                case BrowserType.Firefox:
                    var desiredCapabilities = DesiredCapabilities.Firefox();
                    desiredCapabilities.SetCapability("marionette", false);
                    return desiredCapabilities;
                case BrowserType.FirefoxGecko:
                    return DesiredCapabilities.Firefox();
                case BrowserType.Chrome:
                    return DesiredCapabilities.Chrome();
                case BrowserType.ChromeExtended:
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
                    throw new ArgumentOutOfRangeException(nameof(driverType), driverType, null);
            }
        }
    }

    public enum BrowserType
    {
        Firefox = 1,
        FirefoxGecko,
        Chrome,
        InternetExplorer,
        Opera,
        Safari,
        Phantom,
        Edge,
        ChromeExtended
    }
}
