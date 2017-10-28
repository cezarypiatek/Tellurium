using System;
using OpenQA.Selenium.Remote;
using Tellurium.MvcPages.SeleniumUtils;
using Tellurium.MvcPages.SeleniumUtils.WebDrivers;

namespace Tellurium.MvcPages.BrowserCamera.Lens
{
    public class ChromeFullPageLens : IBrowserCameraLens
    {
        private readonly ChromeDriverExtended driver;

        public ChromeFullPageLens(RemoteWebDriver driver)
        {
            this.driver = driver as ChromeDriverExtended ?? throw new Exception("ChromeFullPageLens works only with BrowserType: ChromeDriverExtended");
        }

        public byte[] TakeScreenshot()
        {
            driver.ScrollToY(0);
            return driver.GetScreenshot().AsByteArray;
        }
    }
}