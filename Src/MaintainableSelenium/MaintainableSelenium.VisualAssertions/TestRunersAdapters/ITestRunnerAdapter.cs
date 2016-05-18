namespace MaintainableSelenium.VisualAssertions.TestRunersAdapters
{
    public interface ITestRunnerAdapter
    {
        bool IsPresent();
        void NotifyAboutTestSuccess(string testName);
        void NotifyAboutTestFail(string testName);
    }
}