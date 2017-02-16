using System;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Remote;
using Tellurium.MvcPages.BrowserCamera.Lens;
using Tellurium.MvcPages.SeleniumUtils;
using Tellurium.MvcPages.Utils;

namespace Tellurium.MvcPages.BrowserCamera
{
    /// <summary>
    /// Responsible for taking screenshots of the page
    /// </summary>
    public class BrowserCamera : IBrowserCamera
    {
        private readonly RemoteWebDriver driver;
        private readonly IBrowserCameraLens lens;

        public BrowserCamera(RemoteWebDriver driver, IBrowserCameraLens lens)
        {
            this.driver = driver;
            this.lens = lens;
        }

        public byte[] TakeScreenshot()
        {
            try
            {
                driver.Blur();
                var currentActiveElement = driver.GetActiveElement();
                MoveMouseOffTheScreen();
                var screenshot = this.lens.TakeScreenshot();
                if (currentActiveElement != null && currentActiveElement.TagName != "body")
                {
                    driver.HoverOn(currentActiveElement);
                }
                return screenshot;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.GetFullExceptionMessage());
                throw;
            }
        }

        private void MoveMouseOffTheScreen()
        {
            try
            {
                var body = driver.FindElementByTagName("body");
                var scrollY = driver.GetScrollY();
                new Actions(driver).MoveToElement(body, 0, scrollY + 1).Perform();
           }
           catch {}
        }

        public static IBrowserCamera CreateNew(RemoteWebDriver driver, BrowserCameraConfig cameraConfig)
        {
            var lens = BrowserCameraLensFactory.Create(driver, cameraConfig.LensType);
            return new BrowserCamera(driver,  lens);
        }
    }
}