using System;
using System.IO;
using JetBrains.TeamCity.ServiceMessages;
using JetBrains.TeamCity.ServiceMessages.Write.Special;
using Tellurium.MvcPages.Utils;
using Tellurium.VisualAssertions.Screenshots.Domain;

namespace Tellurium.VisualAssertions.TestRunnersAdapters.Providers
{
    internal class TeamCityRunnerAdapter : ITestRunnerAdapter
    {
        private const string TeamcityVariableName = "TEAMCITY_PROJECT_NAME";
        private const string VisualAssertionWebPathVariableName = "Tellurium_VisualAssertions_Web";
        private readonly ITeamCityWriter serviceMessage;

        public TeamCityRunnerAdapter(Action<string> testOutputWriter)
        {
            serviceMessage = new TeamCityServiceMessages().CreateWriter(testOutputWriter);
        }

        public static bool IsAvailable()
        {
            return Environment.GetEnvironmentVariable(TeamcityVariableName) != null;
        }

        public void NotifyAboutTestSuccess(string testName, TestSession session, BrowserPattern pattern,
            string testResultMessage)
        {
            var escapedTestName = ServiceMessageReplacements.Encode(testName);
            var testWriter = serviceMessage.OpenTest(escapedTestName);
            testWriter.Dispose();
            UploadReportTabToCI(session, pattern.BrowserName);
        }

        public void NotifyAboutTestFail(string testName, TestSession session, BrowserPattern pattern,
            string testResultMessage)
        {
            var escapedTestName = ServiceMessageReplacements.Encode(testName);
            var detailsMessage = GetDetailsMessage(session, pattern);
            var testWriter = serviceMessage.OpenTest(escapedTestName);
            testWriter.WriteFailed($"Screenshots are different {testResultMessage}", detailsMessage);
            testWriter.Dispose();
            UploadReportTabToCI(session, pattern.BrowserName);
        }

        public void NotifyAboutError(Exception ex)
        {
            serviceMessage.WriteError(ex.GetFullExceptionMessage(), ex.StackTrace);
        }

        private string GetDetailsMessage(TestSession session, BrowserPattern pattern)
        {
            var testResultPreviewPath = GetTestResultPreviewPath(session, pattern);
            return string.IsNullOrWhiteSpace(testResultPreviewPath)? "": $"Details at {testResultPreviewPath}";
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


        private static bool reportUploaded = false;

        private void UploadReportTabToCI(TestSession session, string browserName)
        {
            if (reportUploaded)
            {
                return;
            }
            var visaulAssertionsDashboardUrl = GetVisualAssertionWebPath();
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
            var fullFilePath = Path.Combine(Directory.GetCurrentDirectory(), "VisualAssertions.html");
            File.WriteAllText(fullFilePath, reportContent);
            serviceMessage.PublishArtifact(fullFilePath);
            reportUploaded = true;
        }
    }
}