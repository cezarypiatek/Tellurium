using System;
using Tellurium.VisualAssertions.Screenshots.Domain;

namespace Tellurium.VisualAssertions.TestRunersAdapters
{
    public interface ITestRunnerAdapter
    {
        bool IsPresent();
        void NotifyAboutTestSuccess(string testName, TestSession session, BrowserPattern pattern);
        void NotifyAboutTestFail(string testName, TestSession session, BrowserPattern pattern);
        void NotifyAboutError(Exception ex);
    }
}