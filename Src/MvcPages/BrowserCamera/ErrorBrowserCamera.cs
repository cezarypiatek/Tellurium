using OpenQA.Selenium.Remote;
using Tellurium.MvcPages.BrowserCamera.Lens;
using Tellurium.MvcPages.Utils;

namespace Tellurium.MvcPages.BrowserCamera
{
    internal class ErrorBrowserCamera:IBrowserCamera
    {
        private readonly RegularLens lens;

        public ErrorBrowserCamera(RemoteWebDriver driver)
        {
            this.lens =  new RegularLens(driver);
        }

        public byte[] TakeScreenshot()
        {
            return ExceptionHelper.SwallowException(() =>  lens.TakeScreenshot() , null);
        }
    }
}