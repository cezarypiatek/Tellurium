namespace MaintainableSelenium.MvcPages.BrowserCamera
{
    public interface IScreenshotStorage
    {
        void Persist(byte[] image, string screenshotName);
    }
}