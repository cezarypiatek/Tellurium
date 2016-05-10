using System;
using MaintainableSelenium.MvcPages.BrowserCamera.Lens;
using MaintainableSelenium.MvcPages.SeleniumUtils;
using MaintainableSelenium.MvcPages.Utils;
using OpenQA.Selenium.Remote;

namespace MaintainableSelenium.MvcPages.BrowserCamera
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
                return this.lens.TakeScreenshot();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.GetFullExceptionMessage());
                throw;
            }
        }

        public static IBrowserCamera CreateNew(RemoteWebDriver driver, BrowserCameraConfig cameraConfig)
        {
            var lens = BrowserCameraLensFactory.Create(driver, cameraConfig.LensType);
            return new BrowserCamera(driver,  lens);
        }
    }
}