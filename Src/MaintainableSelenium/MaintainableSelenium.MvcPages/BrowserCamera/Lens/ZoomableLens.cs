using MaintainableSelenium.MvcPages.SeleniumUtils;
using OpenQA.Selenium.Remote;

namespace MaintainableSelenium.MvcPages.BrowserCamera.Lens
{
    public class ZoomableLens:IBrowserCameraLens
    {
        private readonly RemoteWebDriver driver;

        public ZoomableLens(RemoteWebDriver driver)
        {
            this.driver = driver;
        }

        public byte[] TakeScreenshot()
        {
            driver.ZoomToHeight();
            var screenshot = driver.GetScreenshot();
            driver.ResetZoom();
            return screenshot.AsByteArray;
        }
    }
}