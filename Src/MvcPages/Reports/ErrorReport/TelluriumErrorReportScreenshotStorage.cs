using Tellurium.MvcPages.BrowserCamera.Storage;
using Tellurium.MvcPages.Utils;

namespace Tellurium.MvcPages.Reports.ErrorReport
{
    internal class TelluriumErrorReportScreenshotStorage : FileSystemScreenshotStorage
    {
        public TelluriumErrorReportScreenshotStorage(string screenshotDirectoryPath) 
            : base(screenshotDirectoryPath)
        {
        }

        public string PersistErrorScreenshot(byte[] image, string screenshotName)
        {
            base.Persist(image, screenshotName);
            var screenshotPath = GetScreenshotPath(screenshotName);
            if (TeamCityHelpers.IsAvailable())
            {
                return TeamCityHelpers.UploadFileAsArtifact(screenshotPath);
            }

            return screenshotPath;
        }
    }
}