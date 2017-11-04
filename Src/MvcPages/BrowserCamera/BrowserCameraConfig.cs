using Tellurium.MvcPages.BrowserCamera.Lens;

namespace Tellurium.MvcPages.BrowserCamera
{
    public class BrowserCameraConfig
    {
        public LensType LensType { get; set; }

        public static BrowserCameraConfig CreateDefault()
        {
            return new BrowserCameraConfig
            {
                LensType = LensType.Auto
            };
        }
    }
}