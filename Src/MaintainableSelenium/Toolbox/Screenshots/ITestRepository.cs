using System.Collections.Generic;

namespace MaintainableSelenium.Toolbox.Screenshots
{
    public interface ITestRepository
    {
        void SaveTestResult(TestResultInfo testResultInfo);
        void SaveTestCaseInfo(TestCaseInfo testCaseInfo);
        TestCaseInfo GetTestCaseInfo(string testName, string screenshotName, string browserName);
        List<ExtendedTestSessionInfo> GetTestSessions();

        List<TestResultInfo> GetTestsFromSession(string sessionId, string browserName);

        TestCaseInfo GetTestCase(string testCaseId);
        TestResultInfo GetTestResult(string testResultId);
    }
}