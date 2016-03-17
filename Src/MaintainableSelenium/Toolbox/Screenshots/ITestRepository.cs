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
        void AddBlindRegion(string testCaseId, BlindRegion blindRegion);
        void MarkAsPattern(string testResultId);
        List<ExtendedTestCaseInfo> GetTestCases();

        void SaveLocalBlindregions(string testCaseId, List<BlindRegion> localBlindRegions);
        void SaveGlobalBlindregions(string browserName, List<BlindRegion> globalBlindRegions);
        List<BlindRegion> GetGlobalBlindRegions(string browserName);
        void MarkAllAsPattern(string testSessionId, string browserName);
    }
}