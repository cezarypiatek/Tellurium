using System;
using System.IO;
using Tellurium.MvcPages.Utils;

namespace Tellurium.MvcPages.Reports.ErrorReport
{
    internal class TelluriumErrorReportBuilder
    {
        private readonly string reportOutputDir;
        private readonly Action<string> writeOutput;
        private readonly ICIAdapter ciAdapter;
        private const string ReportFileName = "TelluriumErrorReport.html";
        private const string ImagePlaceholder = "<!--Placeholder-->";
        private  string ReportFilePath => Path.Combine(reportOutputDir, ReportFileName);

        public TelluriumErrorReportBuilder(string reportOutputDir, Action<string> writeOutput, ICIAdapter ciAdapter)
        {
            this.reportOutputDir = reportOutputDir;
            this.writeOutput = writeOutput;
            this.ciAdapter = ciAdapter;
        }

        public void ReportException(Exception exception, byte[] errorScreenShot, string screnshotName, string url)
        {
            var storage = new TelluriumErrorReportScreenshotStorage(reportOutputDir, ciAdapter);
            var imgPath = storage.PersistErrorScreenshot(errorScreenShot, screnshotName);
            AppendEntryToReport(exception, url, imgPath);
        }

        public void ReportException(Exception exception, string url)
        {
            AppendEntryToReport(exception, url);
        }

        private void AppendEntryToReport(Exception exception, string url, string imagePath="")
        {
            CreateReportIfNotExists();
            var reportContent = File.ReadAllText(ReportFilePath);
            var newEntry =
                $"<figure><image src=\"{imagePath}\"/><figcaption><p>Error or page <a href=\"{url}\">{url}</a><br/>Reported on <b>{DateTime.Now:G}</b></p><pre>{exception}</pre></figcaption></figure>";
            var newReportContent = reportContent.Replace(ImagePlaceholder, newEntry + ImagePlaceholder);
            File.WriteAllText(ReportFilePath, newReportContent);
            if (ciAdapter.IsAvailable())
            {
                ciAdapter.UploadFileAsArtifact(ReportFilePath);
            }
        }

        private static bool reportInitizlized = false;

        private void CreateReportIfNotExists()
        {
            if (ShouldCreateReportFile())
            {
                File.WriteAllText(ReportFilePath, $"<html><head></head><body><style>img{{max-width:100%}}</style>{ImagePlaceholder}</body></html>");
                writeOutput($"Report created at: {ReportFilePath}");
                reportInitizlized = true;
                if (ciAdapter.IsAvailable())
                {
                    ciAdapter.SetEnvironmentVariable(ReportVariableName, ReportyVariableVal);
                }
                
            }
        }

        private const string ReportVariableName = "TelluriumReportCreated";
        private const string ReportyVariableVal = "true";

        private bool ShouldCreateReportFile()
        {
            if (ciAdapter.IsAvailable() && ciAdapter.GetEnvironmentVariable(ReportVariableName) == ReportyVariableVal)
            {
                return false;
            }
            return File.Exists(ReportFilePath) == false || reportInitizlized == false;
        }
    }

    internal interface ICIAdapter
    {
        bool IsAvailable();
        void SetEnvironmentVariable(string name, string value);
        string GetEnvironmentVariable(string name);
        string UploadFileAsArtifact(string filePath);
    }
}