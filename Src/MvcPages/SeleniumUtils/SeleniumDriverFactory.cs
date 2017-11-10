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
    public class SeleniumDriverFactory
    {
        private readonly BrowserAdapterConfig config;

        public SeleniumDriverFactory(BrowserAdapterConfig config)
        {
            this.config = config;
        }

        public RemoteWebDriver CreateDriver()
        {
            if (config.UseRemoteDriver)
            {
                return CreateRemoteDriver();
            }
            return CreateLocalDriver();
        }

        private static readonly TimeSpan BrowserLoadTimeout = TimeSpan.FromSeconds(60.0);

        RemoteWebDriver CreateLocalDriver()
        {
            switch (this.config.BrowserType)
            {
                case BrowserType.Firefox:
                    var firefoxOptions = CreateFirefoxOptions();
                    var firefoxService = FirefoxDriverService.CreateDefaultService(this.config.SeleniumDriversPath);
                    return new FirefoxDriver(firefoxService, firefoxOptions, BrowserLoadTimeout);
                case BrowserType.FirefoxGecko:
                    var firefoxGeckoOptions = CreateFirefoxGeckoOptions();
                    var firefoxGeckoSerice = FirefoxDriverService.CreateDefaultService(this.config.SeleniumDriversPath);
                    return new FirefoxDriver(firefoxGeckoSerice, firefoxGeckoOptions, BrowserLoadTimeout);
                case BrowserType.FirefoxGeckoHeadless:
                    var firefoxGeckoHeadlessOptions = CreateFirefoxGeckoHeadlessOptions();
                    var firefoxGeckoHeadlessSerice = FirefoxDriverService.CreateDefaultService(this.config.SeleniumDriversPath);
                    return new FirefoxDriver(firefoxGeckoHeadlessSerice, firefoxGeckoHeadlessOptions, BrowserLoadTimeout);
                case BrowserType.Chrome:
                    var chromeOptions = CreateChromeOptions();
                    return new ChromeDriver(this.config.SeleniumDriversPath, chromeOptions);
                case BrowserType.ChromeHeadless:
                    var chromeHeadlessOptions = CreateChromeHeadlessOptions();
                    return new ChromeDriver(this.config.SeleniumDriversPath, chromeHeadlessOptions);
                case BrowserType.InternetExplorer:
                    var internetExplorerOptions = CreateInternetExplorerOptions();
                    return new InternetExplorerDriver(this.config.SeleniumDriversPath, internetExplorerOptions);
                case BrowserType.Opera:
                    var operaOptions = CreateOperaOptions();
                    return new OperaDriver(this.config.SeleniumDriversPath, operaOptions);
                case BrowserType.OperaHeadless:
                    var operaHeadlessOptions = CreateOperaHeadlessOptions();
                    return new OperaDriver(this.config.SeleniumDriversPath, operaHeadlessOptions);
                case BrowserType.Safari:
                    var safariOptions = CreateSafariOptions();
                    var safariService = SafariDriverService.CreateDefaultService(this.config.SeleniumDriversPath);
                    return new SafariDriver(safariService, safariOptions);
                case BrowserType.Phantom:
                    var phantomJsOptions = CreatePhantomJsOptions();
                    return new PhantomJSDriver(this.config.SeleniumDriversPath, phantomJsOptions);
                case BrowserType.Edge:
                    var edgeOptions = CreateEdgeOptions();
                    return new EdgeDriver(this.config.SeleniumDriversPath, edgeOptions);
                default:
                    throw new ArgumentOutOfRangeException(nameof(this.config.BrowserType), this.config.BrowserType, null);
            }
        }

        public RemoteWebDriver CreateRemoteDriver()
        {
            if (string.IsNullOrWhiteSpace(config.SeleniumServerUrl))
            {
                throw new ArgumentException("For remote driver selenium server url is required.");
            }
            var remoteDriverOptions = CreateRemoteDriverOptions();
            return new RemoteWebDriver(new Uri(config.SeleniumServerUrl), remoteDriverOptions);
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

        private OperaOptions CreateOperaOptions()
        {
            var operaOptions = new OperaOptions();
            operaOptions.BinaryLocation = ApplicationHelper.GetOperaBinaryLocation();
            operaOptions.EnableFileDownloading(this.config.DownloadDirPath);
            return operaOptions;
        }

        private OperaOptions CreateOperaHeadlessOptions()
        {
            var options = CreateOperaOptions();
            options.EnableHeadless();
            return options;
        }

        private static InternetExplorerOptions CreateInternetExplorerOptions()
        {
            return new InternetExplorerOptions();
        }

        private ChromeOptions CreateChromeOptions()
        {
            var options = new ChromeOptions();
            options.EnableFileDownloading(this.config.DownloadDirPath);
            return options;
        }

        private ChromeOptions CreateChromeHeadlessOptions()
        {
            var chromeOptions = CreateChromeOptions();
            chromeOptions.EnableHeadless();
            return chromeOptions;
        }

        private FirefoxOptions CreateFirefoxGeckoOptions()
        {
            var firefoxGeckoOptions = new FirefoxOptions
            {
                Profile = new FirefoxProfile {DeleteAfterUse = true}
            };
            firefoxGeckoOptions.Profile.EnableFileDownloading(this.config.DownloadDirPath, config.AllowedMimeTypes);
            return firefoxGeckoOptions;
        }

        private FirefoxOptions CreateFirefoxGeckoHeadlessOptions()
        {
            var geckoOptions = CreateFirefoxGeckoOptions();
            geckoOptions.EnableHeadless();
            return geckoOptions;
        }

        private FirefoxOptions CreateFirefoxOptions()
        {
            var firefoxOptions = new FirefoxOptions
            {
                Profile = new FirefoxProfile {DeleteAfterUse = true},
                UseLegacyImplementation = true
            };
            firefoxOptions.Profile.EnableFileDownloading(this.config.DownloadDirPath, this.config.AllowedMimeTypes);
            return firefoxOptions;
        }

        private DriverOptions CreateRemoteDriverOptions()
        {
            switch (config.BrowserType)
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
                    throw new ArgumentOutOfRangeException(nameof(this.config.BrowserType), this.config.BrowserType, null);
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
