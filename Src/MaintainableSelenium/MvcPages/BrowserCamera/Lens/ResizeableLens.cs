using System.Drawing;
using OpenQA.Selenium.Remote;
using Tellurium.MvcPages.SeleniumUtils;

namespace Tellurium.MvcPages.BrowserCamera.Lens
{
    public class ResizeableLens:IBrowserCameraLens
    {
        private readonly RemoteWebDriver driver;

        public ResizeableLens(RemoteWebDriver driver)
        {
            this.driver = driver;
        }

        public byte[] TakeScreenshot()
        {
            var pageHeight = driver.GetPageHeight();
            var viewPortHeight = driver.GetWindowHeight();
            var window = driver.Manage().Window;
            var originalSize = window.Size;
            if (viewPortHeight < pageHeight)
            {
                window.Size = new Size
                {
                    Height = pageHeight,
                    Width = originalSize.Width
                };
            }
            var screenshot = driver.GetScreenshot();
            window.Size = originalSize;
            return screenshot.AsByteArray;
        }
    }
}