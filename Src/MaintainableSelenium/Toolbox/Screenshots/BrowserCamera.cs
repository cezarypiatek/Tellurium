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
        
        public BrowserCamera(RemoteWebDriver driver, string imageNamePrefix, ScreenshotService screenshotService)
        {
            this.driver = driver;
            this.imageNamePrefix = imageNamePrefix;
            this.screenshotService = screenshotService;
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