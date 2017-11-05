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
                    var firefoxOptions = CreateFirefoxOptions();
                    var firefoxService = FirefoxDriverService.CreateDefaultService(driversPath);
                    return new FirefoxDriver(firefoxService, firefoxOptions, BrowserLoadTimeout);
                case BrowserType.FirefoxGecko:
                    var firefoxGeckoOptions = CreateFirefoxGeckoOptions();
                    var firefoxGeckoSerice = FirefoxDriverService.CreateDefaultService(driversPath);
                    return new FirefoxDriver(firefoxGeckoSerice, firefoxGeckoOptions, BrowserLoadTimeout);
                case BrowserType.FirefoxGeckoHeadless:
                    var firefoxGeckoHeadlessOptions = CreateFirefoxGeckoHeadlessOptions();
                    var firefoxGeckoHeadlessSerice = FirefoxDriverService.CreateDefaultService(driversPath);
                    return new FirefoxDriver(firefoxGeckoHeadlessSerice, firefoxGeckoHeadlessOptions, BrowserLoadTimeout);
                case BrowserType.Chrome:
                    var chromeOptions = CreateChromeOptions();
                    return new ChromeDriver(driversPath, chromeOptions);
                case BrowserType.ChromeHeadless:
                    var chromeHeadlessOptions = CreateChromeHeadlessOptions();
                    return new ChromeDriver(driversPath, chromeHeadlessOptions);
                case BrowserType.InternetExplorer:
                    var internetExplorerOptions = CreateInternetExplorerOptions();
                    return new InternetExplorerDriver(driversPath, internetExplorerOptions);
                case BrowserType.Opera:
                    var operaOptions = CreateOperaOptions();
                    return new OperaDriver(driversPath, operaOptions);
                case BrowserType.OperaHeadless:
                    var operaHeadlessOptions = CreateOperaHeadlessOptions();
                    return new OperaDriver(driversPath, operaHeadlessOptions);
                case BrowserType.Safari:
                    var safariOptions = CreateSafariOptions();
                    var safariService = SafariDriverService.CreateDefaultService(driversPath);
                    return new SafariDriver(safariService, safariOptions);
                case BrowserType.Phantom:
                    var phantomJsOptions = CreatePhantomJsOptions();
                    return new PhantomJSDriver(driversPath, phantomJsOptions);
                case BrowserType.Edge:
                    var edgeOptions = CreateEdgeOptions();
                    return new EdgeDriver(driversPath, edgeOptions);
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
            var remoteDriverOptions = CreateRemoteDriverOptions(driverType);
            return new RemoteWebDriver(new Uri(seleniumServerUrl), remoteDriverOptions);
        }

        private static EdgeOptions CreateEdgeOptions()
        {
            return new EdgeOptions();
        }

        private static PhantomJSOptions CreatePhantomJsOptions()
        {
            return new PhantomJSOptions();
        }

        private static SafariOptions CreateSafariOptions()
        {
            return new SafariOptions();
        }

        private static OperaOptions CreateOperaOptions()
        {
            var operaOptions = new OperaOptions();
            return operaOptions;
        }

        private static OperaOptions CreateOperaHeadlessOptions()
        {
            var options = CreateOperaOptions();
            options.AddArguments("headless");
            options.AddArgument("disable-gpu");
            return options;
        }

        private static InternetExplorerOptions CreateInternetExplorerOptions()
        {
            return new InternetExplorerOptions();
        }

        private static ChromeOptions CreateChromeOptions()
        {
            return new ChromeOptions();
        }

        private static ChromeOptions CreateChromeHeadlessOptions()
        {
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("headless");
            chromeOptions.AddArgument("disable-gpu");
            return chromeOptions;
        }

        private static FirefoxOptions CreateFirefoxGeckoOptions()
        {
            return new FirefoxOptions
            {
                Profile = new FirefoxProfile {DeleteAfterUse = true}
            };
        }

        private static FirefoxOptions CreateFirefoxGeckoHeadlessOptions()
        {
            var geckoOptions = CreateFirefoxGeckoOptions();
            geckoOptions.AddArgument("--headless");
            return geckoOptions;
        }

        private static FirefoxOptions CreateFirefoxOptions()
        {
            return new FirefoxOptions
            {
                Profile = new FirefoxProfile {DeleteAfterUse = true},
                UseLegacyImplementation = true
            };
        }

        private static DriverOptions CreateRemoteDriverOptions(BrowserType driverType)
        {
            switch (driverType)
            {
                case BrowserType.Firefox:
                    return CreateFirefoxOptions();
                case BrowserType.FirefoxGecko:
                    return CreateFirefoxGeckoOptions();
                case BrowserType.FirefoxGeckoHeadless:
                    return CreateFirefoxGeckoHeadlessOptions();
                case BrowserType.Chrome:
                    return CreateChromeOptions();
                case BrowserType.InternetExplorer:
                    return CreateInternetExplorerOptions();
                case BrowserType.Opera:
                    return CreateOperaOptions();
                case BrowserType.OperaHeadless:
                    return CreateOperaHeadlessOptions();
                case BrowserType.Safari:
                    return CreateSafariOptions();
                case BrowserType.Phantom:
                    return CreatePhantomJsOptions();
                case BrowserType.Edge:
                    return CreateEdgeOptions();
                case BrowserType.ChromeHeadless:
                    return CreateChromeHeadlessOptions();
                default:
                    throw new ArgumentOutOfRangeException(nameof(driverType), driverType, null);
            }
        }
    }

    public enum BrowserType
    {
        Firefox = 1,
        FirefoxGecko,
        FirefoxGeckoHeadless,
        Chrome,
        ChromeHeadless,
        InternetExplorer,
        Opera,
        OperaHeadless,
        Safari,
        Phantom,
        Edge
    }
}
