using System;

namespace Tellurium.MvcPages.BrowserCamera.Storage
{
    public class TeamCityScreenshotStorage:FileSystemScreenshotStorage
    {
        public TeamCityScreenshotStorage() 
            : base(Environment.CurrentDirectory)
        {
        }

        public override void Persist(byte[] image, string screenshotName)
        {
            base.Persist(image, screenshotName);
            var screenshotPath = GetScreenshotPath(screenshotName);
            UploadFileAsArtifact(screenshotPath);
        }

        private void UploadFileAsArtifact(string filePath)
        {
            Console.WriteLine($"##teamcity[publishArtifacts '{filePath}']");
        }


        public static bool IsAvailable()
        {
            return Environment.GetEnvironmentVariable(TeamcityVariableName) != null;
        }

        private const string TeamcityVariableName = "TEAMCITY_PROJECT_NAME";
    }
}