namespace MaintainableSelenium.Toolbox.Screenshots
{
    public interface ITestRunnerAdapter
    {
        TestSessionInfo GetTestSessionInfo();
        string GetCurrentTestName();
    }
}