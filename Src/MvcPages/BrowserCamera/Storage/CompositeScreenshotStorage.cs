namespace Tellurium.MvcPages.BrowserCamera.Storage
{
    public class CompositeScreenshotStorage:IScreenshotStorage
    {
        private readonly IScreenshotStorage[] storages;

        public CompositeScreenshotStorage(params IScreenshotStorage[] storages)
        {
            this.storages = storages;
        }

        public void Persist(byte[] image, string screenshotName)
        {
            foreach (var storage in storages)
            {
                storage.Persist(image, screenshotName);
            }
        }
    }
}