namespace MaintainableSelenium.Toolbox.Screenshots
{
    public interface ITestRunnerAdapter
    {
        TestSession GetTestSessionInfo();
        string GetCurrentTestName();
    }
}