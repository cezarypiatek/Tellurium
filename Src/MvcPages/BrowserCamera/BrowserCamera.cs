using System;
using OpenQA.Selenium;
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
                IWebElement currentActiveElement = null;

                ExceptionHelper.SwallowException(() =>
                {
                    driver.Blur();
                    currentActiveElement = driver.GetActiveElement();
                    driver.MoveMouseOffTheScreen();
                });
                
                var screenshot = this.lens.TakeScreenshot();
                ExceptionHelper.SwallowException(() =>
                {
                    if (currentActiveElement != null && currentActiveElement.TagName != "body")
                    {
                        driver.HoverOn(currentActiveElement);
                    }
                });
                
                return screenshot;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.GetFullExceptionMessage());
                throw;
            }
        }
    }
}