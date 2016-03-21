using System.Collections.Generic;

namespace MaintainableSelenium.Toolbox.Screenshots
{
    public interface ITestRepository
    {
        void SaveTestResult(TestResultInfo testResultInfo);
        TestResultInfo GetTestResult(string testResultId);
        List<TestResultInfo> GetTestsFromSession(string sessionId, string browserName);
        void MarkAsPattern(string testResultId);
        void MarkAllAsPattern(string testSessionId, string browserName);
    }

    public interface ITestCaseRepository
    {
        void Save(TestCase testCase);
        TestCase Find(string testName, string screenshotName, string browserName);
        TestCase Get(string testCaseId);
        List<ExtendedTestCaseInfo> GetTestCases();
        void SaveLocalBlindregions(string testCaseId, List<BlindRegion> localBlindRegions);
        void SaveGlobalBlindregions(string browserName, List<BlindRegion> globalBlindRegions);
        List<BlindRegion> GetGlobalBlindRegions(string browserName);
    }

    public interface ITestSessionRepository
    {
        void Save(TestSessionInfo testSession);
        List<ExtendedTestSessionInfo> GetTestSessions();
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