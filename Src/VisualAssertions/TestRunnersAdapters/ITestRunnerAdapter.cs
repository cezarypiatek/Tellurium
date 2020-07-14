using System;
using Tellurium.VisualAssertions.Screenshots.Domain;

namespace Tellurium.VisualAssertions.TestRunnersAdapters
{
    public interface ITestRunnerAdapter
    {
        void NotifyAboutTestSuccess(string testName, TestSession session, BrowserPattern pattern,
            string testResultMessage);
        void NotifyAboutTestFail(string testName, TestSession session, BrowserPattern pattern,
            string testResultMessage);
        void NotifyAboutError(Exception ex);
    }
}