using MaintainableSelenium.MvcPages.BrowserCamera.Lens;

namespace MaintainableSelenium.MvcPages.BrowserCamera
{
    public class BrowserCameraConfig
    {
        public LensType LensType { get; set; }

        public static BrowserCameraConfig CreateDefault()
        {
            return new BrowserCameraConfig
            {
                LensType = LensType.Regular
            };
        }
    }
}