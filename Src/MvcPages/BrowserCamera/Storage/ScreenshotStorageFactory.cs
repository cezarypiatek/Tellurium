using Tellurium.MvcPages.Configuration;

namespace Tellurium.MvcPages.BrowserCamera.Storage
{
    internal static class ScreenshotStorageFactory
    {
        public static IScreenshotStorage CreateForErrorScreenshot(BrowserAdapterConfig adapterConfig)
        {
            if (string.IsNullOrWhiteSpace(adapterConfig.ErrorScreenshotsPath) == false)
                return new FileSystemScreenshotStorage(adapterConfig.ErrorScreenshotsPath);
            return null;
        }
    }
}