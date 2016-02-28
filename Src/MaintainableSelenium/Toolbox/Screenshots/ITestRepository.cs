namespace MaintainableSelenium.Toolbox.Screenshots
{
    public interface ITestRepository
    {
        void SaveTestResult(TestResultInfo testResultInfo);
        void SaveTestCaseInfo(TestCaseInfo testCaseInfo);
        TestCaseInfo GetTestCaseInfo(string testName, string screenshotName, string browserName);
    }
}