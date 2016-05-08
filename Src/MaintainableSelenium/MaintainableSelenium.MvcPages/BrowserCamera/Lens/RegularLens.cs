using OpenQA.Selenium.Remote;

namespace MaintainableSelenium.MvcPages.BrowserCamera.Lens
{
    public class RegularLens: IBrowserCameraLens
    {
        private readonly RemoteWebDriver driver;

        public RegularLens(RemoteWebDriver driver)
        {
            this.driver = driver;
        }

        public byte[] TakeScreenshot()
        {
            return driver.GetScreenshot().AsByteArray;
        }
    }
}