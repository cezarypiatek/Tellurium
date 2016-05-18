using System;

namespace MaintainableSelenium.VisualAssertions.TestRunersAdapters.Providers
{
    public class TeamCityRunnerAdapter : ITestRunnerAdapter
    {
        private const string TeamcityVariableName = "TEAMCITY_PROJECT_NAME";

        public bool IsPresent()
        {
            return Environment.GetEnvironmentVariable(TeamcityVariableName) != null;
        }

        public void NotifyAboutTestSuccess(string testName)
        {
            var escapedTestName = Escape(testName);
            Console.WriteLine("##teamcity[testStarted name='{0}']", escapedTestName);
            Console.WriteLine("##teamcity[testFinished name='{0}']", escapedTestName);
        }

        public void NotifyAboutTestFail(string testName)
        {
            var escapedTestName = Escape(testName);
            Console.WriteLine("##teamcity[testStarted name='{0}']", escapedTestName);
            Console.WriteLine("##teamcity[testFailed name='{0}' details='{1}']", escapedTestName, "Screenshots are different");
            Console.WriteLine("##teamcity[testFinished name='{0}']", escapedTestName);
        }

        static string Escape(string value)
        {
            return value.Replace("|", "||")
                .Replace("'", "|'")
                .Replace("\r", "|r")
                .Replace("\n", "|n")
                .Replace("]", "|]");
        }
    }
}