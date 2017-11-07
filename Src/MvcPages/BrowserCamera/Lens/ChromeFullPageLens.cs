using System;
using OpenQA.Selenium.Remote;
using Tellurium.MvcPages.SeleniumUtils;
using Tellurium.MvcPages.SeleniumUtils.ChromeRemoteInterface;

namespace Tellurium.MvcPages.BrowserCamera.Lens
{
    public class ChromeFullPageLens : IBrowserCameraLens
    {
        private readonly RemoteWebDriver driver;
        private readonly ChromeRemoteInterface chromeRemoteInterface;

        public ChromeFullPageLens(RemoteWebDriver driver)
        {
            if (IsSupported(driver) == false)
            {
                throw new Exception("ChromeFullPageLens works only with Chrome version 59 or higher");
            }

            this.driver = driver;
            this.chromeRemoteInterface = new ChromeRemoteInterface(driver);
        }

        public static bool IsSupported(RemoteWebDriver driver)
        {
            if (Version.TryParse(driver.Capabilities.Version, out var version))
            {
                return string.Equals(driver.Capabilities.BrowserName, "chrome", StringComparison.OrdinalIgnoreCase) && version.Major >= 59;
            }
            return false;
        }

        public byte[] TakeScreenshot()
        {
            driver.ScrollToY(0);
            chromeRemoteInterface.SetDeviceMetricsForFullPage();
            try
            {
                return chromeRemoteInterface.CaptureScreenshot();
            }
            finally
            {
                chromeRemoteInterface.ClearDeviceMetrics();
            }
        }
    }
}