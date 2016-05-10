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

        public void Persist(byte[] image, string screenshotName)
        {
            var fileName = string.Format("{0}.jpg", screenshotName);
            var screenshotPath = Path.Combine(screenshotDirectoryPath, fileName);
            image.ToBitmap().Save(screenshotPath, ImageFormat.Jpeg);
        }
    }
}