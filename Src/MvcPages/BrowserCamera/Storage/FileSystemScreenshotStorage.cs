using System;
using System.Drawing.Imaging;
using System.IO;
using Tellurium.MvcPages.Utils;

namespace Tellurium.MvcPages.BrowserCamera.Storage
{
    public class FileSystemScreenshotStorage : IScreenshotStorage
    {
        private readonly string screenshotDirectoryPath;

        public FileSystemScreenshotStorage(string screenshotDirectoryPath)
        {
            this.screenshotDirectoryPath = screenshotDirectoryPath;
        }

        public virtual void Persist(byte[] image, string screenshotName)
        {
            var screenshotPath = GetScreenshotPath(screenshotName);
            image.ToBitmap().Save(screenshotPath, ImageFormat.Jpeg);
        }

        protected string GetScreenshotPath(string screenshotName)
        {
            if (string.IsNullOrWhiteSpace(screenshotDirectoryPath))
            {
                throw new ApplicationException("Screenshot directory path not defined");
            }

            if (string.IsNullOrWhiteSpace(screenshotName))
            {
                throw new ArgumentException("Screenshot name cannot be empty", nameof(screenshotName));
            }
            var fileName = $"{screenshotName}.jpg";
            return Path.Combine(screenshotDirectoryPath, fileName);
        }
    }
}