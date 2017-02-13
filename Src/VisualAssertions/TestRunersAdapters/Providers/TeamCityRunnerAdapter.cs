using System;
using System.IO;
using Tellurium.MvcPages.Utils;
using Tellurium.VisualAssertions.Screenshots.Domain;

namespace Tellurium.VisualAssertions.TestRunersAdapters.Providers
{
    internal class TeamCityRunnerAdapter : ITestRunnerAdapter
    {
        private const string TeamcityVariableName = "TEAMCITY_PROJECT_NAME";
        public const string VisualAssertionWebPathVariableName = "Tellurium_VisualAssertions_Web";

        public bool IsPresent()
        {
            return Environment.GetEnvironmentVariable(TeamcityVariableName) != null;
        }

        public void NotifyAboutTestSuccess(string testName, TestSession session, BrowserPattern pattern)
        {
            var escapedTestName = Escape(testName);
            Console.WriteLine("##teamcity[testStarted name='{0}']", escapedTestName);
            Console.WriteLine("##teamcity[testFinished name='{0}']", escapedTestName);
            UploadReportTabToCI(session, pattern.BrowserName);
        }

        public void NotifyAboutTestFail(string testName, TestSession session, BrowserPattern pattern)
        {
            var escapedTestName = Escape(testName);
            var detailsMessage = GetDetailsMessage(session, pattern);
            Console.WriteLine("##teamcity[testStarted name='{0}']", escapedTestName);
            Console.WriteLine("##teamcity[testFailed name='{0}' details='{1}']", escapedTestName, detailsMessage);
            Console.WriteLine("##teamcity[testFinished name='{0}']", escapedTestName);
            UploadReportTabToCI(session, pattern.BrowserName);
        }

        public void NotifyAboutError(Exception ex)
        {
            Console.WriteLine("##teamcity[message text='{0}' errorDetails='{1}' status='ERROR']",ex.GetFullExceptionMessage(), ex.StackTrace);
        }

        private string GetDetailsMessage(TestSession session, BrowserPattern pattern)
        {
            var testResultPreviewPath = GetTestResultPreviewPath(session, pattern);
            var detailsInfo = string.IsNullOrWhiteSpace(testResultPreviewPath)? "": $"Details at {testResultPreviewPath}";
            return $"Screenshots are different. {detailsInfo}";
        }

        private string GetVisualAssertionWebPath()
        {
            return Environment.GetEnvironmentVariable(VisualAssertionWebPathVariableName)?.TrimEnd('/');
        }

        private string GetTestResultPreviewPath(TestSession session, BrowserPattern pattern)
        {
            var rootPath = GetVisualAssertionWebPath();
            if (string.IsNullOrWhiteSpace(rootPath))
            {
                return string.Empty;
            }
            return $"{rootPath}/Home/GetTestResultPreview?testSessionId={session.Id}&patternId={pattern.Id}";
        }

        static string Escape(string value)
        {
            return value.Replace("|", "||")
                .Replace("'", "|'")
                .Replace("\r", "|r")
                .Replace("\n", "|n")
                .Replace("[", "|[")
                .Replace("]", "|]");
        }


        private static bool reportUploaded = false;

        private void UploadReportTabToCI(TestSession session, string browserName)
        {
            if (TeamCityHelpers.IsAvailable() == false)
            {
                return;
            }

            if (reportUploaded)
            {
                return;
            }
            var visaulAssertionsDashboardUrl = TeamCityHelpers.GetTeamcityVariable(VisualAssertionWebPathVariableName);
            if (string.IsNullOrWhiteSpace(visaulAssertionsDashboardUrl))
            {
                return;
            }

            var reportContent = $@"<html>
<head>
<script>
window.location = ""{visaulAssertionsDashboardUrl}/Home/GetTestsFromSessionSession?sessionId={session.Id}&browserName={browserName}"";
</script>
</head><body></body>
</html> ";

            File.WriteAllText("VisualAssertions.html", reportContent);
            TeamCityHelpers.UploadFileAsArtifact("VisualAssertions.html");
            reportUploaded = true;
        }
    }
}