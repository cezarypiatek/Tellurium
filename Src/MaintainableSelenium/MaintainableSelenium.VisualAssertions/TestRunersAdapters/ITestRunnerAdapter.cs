using System;
using MaintainableSelenium.VisualAssertions.Screenshots.Domain;

namespace MaintainableSelenium.VisualAssertions.TestRunersAdapters
{
    public interface ITestRunnerAdapter
    {
        bool IsPresent();
        void NotifyAboutTestSuccess(string testName);
        void NotifyAboutTestFail(string testName, TestSession session, BrowserPattern pattern);
        void NotifyAboutError(Exception ex);
    }
}