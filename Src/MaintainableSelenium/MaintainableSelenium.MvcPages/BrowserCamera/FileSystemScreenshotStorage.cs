using System.Drawing.Imaging;
using System.IO;
using MaintainableSelenium.MvcPages.Utils;

namespace MaintainableSelenium.MvcPages.BrowserCamera
{
    public class FileSystemScreenshotStorage : IScreenshotStorage
    {
        private readonly string screenshotDirectoryPath;

        public FileSystemScreenshotStorage(string screenshotDirectoryPath)
        {
            this.screenshotDirectoryPath = screenshotDirectoryPath;
        }

        public void Persist(byte[] image, ScreenshotIdentity screenshotIdentity)
        {
            var screenshotFileName = GetScreenshotFileName(screenshotIdentity);
            var screenshotPath = Path.Combine(screenshotDirectoryPath, screenshotFileName);
            image.ToBitmap().Save(screenshotPath, ImageFormat.Jpeg);
        }

        private static string GetScreenshotFileName(ScreenshotIdentity screenshotIdentity)
        {
            return string.Format("{0}_{1}_{2}_{3}.png", screenshotIdentity.ProjectName, screenshotIdentity.BrowserName, screenshotIdentity.Category, screenshotIdentity.ScreenshotName);
        }
    }
}