using System;
using System.Drawing.Imaging;
using System.IO;
using Tellurium.MvcPages.Utils;

namespace Tellurium.MvcPages.BrowserCamera
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
            if (string.IsNullOrWhiteSpace(screenshotDirectoryPath))
            {
                throw new ApplicationException("Screenshot directory path not defined");
            }

            if (string.IsNullOrWhiteSpace(screenshotName))
            {
                throw new ArgumentException("Screenshot name cannot be empty", "screenshotName");
            }

            var fileName = string.Format("{0}.jpg", screenshotName);
            var screenshotPath = Path.Combine(screenshotDirectoryPath, fileName);
            image.ToBitmap().Save(screenshotPath, ImageFormat.Jpeg);
        }
    }
}