using MaintainableSelenium.MvcPages.BrowserCamera.Lens;

namespace MaintainableSelenium.MvcPages.BrowserCamera
{
    public class BrowserCameraConfig
    {
        public string ProjectName { get; set; }
        public string ScreenshotCategory { get; set; }
        public LensType LensType { get; set; }
        public IScreenshotStorage ScreenshotStorage { get; set; }
    }
}