using System.Collections.Generic;

namespace MaintainableSelenium.Toolbox.Screenshots
{
    public interface ITestRepository
    {
        void SaveTestResult(TestResultInfo testResultInfo);
        void SaveTestCaseInfo(TestCase testCase);
        TestCase GetTestCaseInfo(string testName, string screenshotName, string browserName);
        List<ExtendedTestSessionInfo> GetTestSessions();

        List<TestResultInfo> GetTestsFromSession(string sessionId, string browserName);

        TestCase GetTestCase(string testCaseId);
        TestResultInfo GetTestResult(string testResultId);
        void AddBlindRegion(string testCaseId, BlindRegion blindRegion);
        void MarkAsPattern(string testResultId);
        List<ExtendedTestCaseInfo> GetTestCases();

        void SaveLocalBlindregions(string testCaseId, List<BlindRegion> localBlindRegions);
        void SaveGlobalBlindregions(string browserName, List<BlindRegion> globalBlindRegions);
        List<BlindRegion> GetGlobalBlindRegions(string browserName);
        void MarkAllAsPattern(string testSessionId, string browserName);
    }

    public class TestCaseSet
    {
        public string Id { get; set; }
        public string Browser { get; set; }
        public List<BlindRegion> GlobalBlindRegions { get; set; }
        public List<TestCase> TestCases { get; set; }
    }

    public class Project
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<TestCaseSet> TestCaseSets { get; set; }
        public List<TestSessionInfo> Sessions { get; set; }
    }
}