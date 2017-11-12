using System;
using OpenQA.Selenium.Remote;

namespace Tellurium.MvcPages.BrowserCamera.Lens
{
    public static class BrowserCameraLensFactory
    {
        public static IBrowserCameraLens Create(RemoteWebDriver webDriver, LensType type)
        {
            switch (type)
            {
                case LensType.Auto:
                    return CreateLensForAutoMode(webDriver);
                case LensType.Regular:
                    return new RegularLens(webDriver);
                case LensType.Scrollable:
                    return new ScrollableLens(webDriver);
                case LensType.Resizeable:
                    return new ResizeableLens(webDriver);
                case LensType.Zoomable:
                    return new ZoomableLens(webDriver);
                case LensType.ChromeFullPage:
                    return new ChromeFullPageLens(webDriver);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public static IBrowserCameraLens CreateLensForAutoMode(RemoteWebDriver webDriver)
        {
            if (ChromeFullPageLens.IsSupported(webDriver))
            {
                return new ChromeFullPageLens(webDriver);
            }

            if (IsHeadlessFirefox(webDriver))
            {
                return new ResizeableLens(webDriver);
            }
            return new RegularLens(webDriver);
        }

        private static bool IsHeadlessFirefox(RemoteWebDriver webDriver)
        {
            return webDriver.Capabilities.BrowserName == "firefox" && (bool) webDriver.Capabilities.GetCapability("moz:headless") == true;
        }
    }

    public enum LensType
    {
        Auto =1,
        Regular,
        Scrollable,
        Resizeable,
        Zoomable,
        ChromeFullPage
    }
}