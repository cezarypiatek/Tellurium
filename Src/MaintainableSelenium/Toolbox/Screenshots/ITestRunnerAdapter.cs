namespace MaintainableSelenium.Toolbox.Screenshots
{
    public interface ITestRunnerAdapter
    {
        string GetTestSessionId();
        string GetCurrentTestName();
    }
}