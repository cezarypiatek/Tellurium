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
    }

    public enum LensType
    {
        Regular = 1,
        Scrollable,
        Resizeable,
        Zoomable,
        ChromeFullPage
    }
}