using OpenQA.Selenium.Remote;
using Tellurium.MvcPages.BrowserCamera.Lens;

namespace Tellurium.MvcPages.BrowserCamera
{
    internal static class BrowserCameraFactory
    {
        public static IBrowserCamera CreateNew(RemoteWebDriver driver, BrowserCameraConfig cameraConfig)
        {
            var lens = BrowserCameraLensFactory.Create(driver, cameraConfig.LensType);
            return new BrowserCamera(driver, lens);
        }

        public static IBrowserCamera CreateErrorBrowserCamera(RemoteWebDriver driver)
        {
            return new ErrorBrowserCamera(driver);
        }
    }
}