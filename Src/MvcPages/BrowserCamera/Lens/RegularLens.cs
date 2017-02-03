using OpenQA.Selenium.Remote;
using Tellurium.MvcPages.SeleniumUtils;

namespace Tellurium.MvcPages.BrowserCamera.Lens
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
            driver.ScrollToY(0);
            return driver.GetScreenshot().AsByteArray;
        }
    }
}