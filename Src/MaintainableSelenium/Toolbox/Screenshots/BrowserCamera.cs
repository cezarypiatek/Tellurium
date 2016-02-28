using System.Collections.Generic;
using OpenQA.Selenium.Remote;

namespace MaintainableSelenium.Toolbox.Screenshots
{
    /// <summary>
    /// Responsible for taking screenshots of the page
    /// </summary>
    public class BrowserCamera : IBrowserCamera
    {
        private readonly RemoteWebDriver driver;
        private readonly string imageNamePrefix;
        private readonly ScreenshotService screenshotService;
        
        private readonly List<BlindRegion> blindRegions;
        public BrowserCamera(RemoteWebDriver driver, string imageNamePrefix, ScreenshotService screenshotService, List<BlindRegion> blindRegions = null)
        {
            this.driver = driver;
            this.imageNamePrefix = imageNamePrefix;
            this.screenshotService = screenshotService;
            this.blindRegions = blindRegions ?? new List<BlindRegion>();
        }

        public void TakeScreenshot(string name)
        {
            driver.Blur();
            var screenshot = driver.GetScreenshot();
            var fullName = string.Format("{0}_{1}", imageNamePrefix, name);
            screenshotService.Persist(fullName, driver.Capabilities.BrowserName, screenshot.AsByteArray);
        }
    }
}