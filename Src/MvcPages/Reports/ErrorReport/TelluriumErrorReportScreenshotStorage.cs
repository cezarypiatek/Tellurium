using Tellurium.MvcPages.BrowserCamera.Storage;
using Tellurium.MvcPages.Utils;

namespace Tellurium.MvcPages.Reports.ErrorReport
{
    internal class TelluriumErrorReportScreenshotStorage : FileSystemScreenshotStorage
    {
        private readonly ICIAdapter ciAdapter;

        public TelluriumErrorReportScreenshotStorage(string screenshotDirectoryPath, ICIAdapter ciAdapter) 
            : base(screenshotDirectoryPath)
        {
            this.ciAdapter = ciAdapter;
        }

        public string PersistErrorScreenshot(byte[] image, string screenshotName)
        {
            base.Persist(image, screenshotName);
            var screenshotPath = GetScreenshotPath(screenshotName);
            if (ciAdapter.IsAvailable())
            {
                return ciAdapter.UploadFileAsArtifact(screenshotPath);
            }

            return screenshotPath;
        }
    }
}