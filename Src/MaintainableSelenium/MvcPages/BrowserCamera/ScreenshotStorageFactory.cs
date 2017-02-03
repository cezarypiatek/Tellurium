namespace Tellurium.MvcPages.BrowserCamera
{
    public static class ScreenshotStorageFactory
    {
        public static IScreenshotStorage CreateFileSystemStorage(string screenshotDirectoryPath)
        {
            return new FileSystemScreenshotStorage(screenshotDirectoryPath);
        }
    }
}