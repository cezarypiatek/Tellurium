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

        private static IBrowserCameraLens CreateLensForAutoMode(RemoteWebDriver webDriver)
        {
            if (ChromeFullPageLens.IsSupported(webDriver))
            {
                return new ChromeFullPageLens(webDriver);
            }
            return new RegularLens(webDriver);
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