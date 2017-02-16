using System;
using System.IO;
using Tellurium.MvcPages.Utils;

namespace Tellurium.MvcPages.Reports.ErrorReport
{
    internal class TelluriumErrorReportBuilder
    {
        private readonly string reportOutputDir;
        private const string ReportFileName = "TelluriumErrorReport.html";
        private const string ImagePlaceholder = "<!--Placeholder-->";
        private  string ReportFilePath => Path.Combine(reportOutputDir, ReportFileName);

        public TelluriumErrorReportBuilder(string reportOutputDir=null)
        {
            this.reportOutputDir = string.IsNullOrWhiteSpace(reportOutputDir)? Environment.CurrentDirectory: reportOutputDir;
        }

        public void ReportException(Exception exception, byte[] errorScreenShot, string screnshotName)
        {
            var storage = new TelluriumErrorReportScreenshotStorage(reportOutputDir);
            var imgPath = storage.PersistErrorScreenshot(errorScreenShot, screnshotName);
            AppendImageToReport(imgPath, $"{exception.Message}\r\n{exception.StackTrace}");
        }

        public void ReportException(Exception exception)
        {
            AppendImageToReport("", $"{exception.Message}\r\n{exception.StackTrace}");
        }

        private void AppendImageToReport(string imagePath, string description)
        {
            CreateReportIfNotExists();
            var reportContent = File.ReadAllText(ReportFilePath);
            var newEntry =
                $"<figure><image src=\"{imagePath}\"/><figcaption><pre>{description}</pre></figcaption></figure>";
            var newReportContent = reportContent.Replace(ImagePlaceholder, newEntry + ImagePlaceholder);
            File.WriteAllText(ReportFilePath, newReportContent);
            if (TeamCityHelpers.IsAvailable())
            {
                TeamCityHelpers.UploadFileAsArtifact(ReportFilePath);
            }
        }

        private static bool reportInitizlized = false;

        private void CreateReportIfNotExists()
        {
            if (ShouldCreateReportFile())
            {
                File.WriteAllText(ReportFilePath, $"<html><head></head><body><style>img{{max-width:100%}}</style>{ImagePlaceholder}</body></html>");
                Console.WriteLine($"Report created at: {ReportFilePath}");
                reportInitizlized = true;
                if (TeamCityHelpers.IsAvailable())
                {
                    TeamCityHelpers.SetTeamcityEnvironmentVariable(TeamCityTelluriumReportVariableName, "true");
                }
                
            }
        }

        private const string TeamCityTelluriumReportVariableName = "TelluriumReportCreated";

        private bool ShouldCreateReportFile()
        {
            if (TeamCityHelpers.IsAvailable() && TeamCityHelpers.GetTeamcityVariable(TeamCityTelluriumReportVariableName) != "true")
            {
                return false;
            }
            return File.Exists(ReportFilePath) == false || reportInitizlized == false;
        }
    }
}