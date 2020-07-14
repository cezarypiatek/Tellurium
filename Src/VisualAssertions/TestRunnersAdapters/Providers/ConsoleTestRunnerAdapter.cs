using System;
using Tellurium.MvcPages.Utils;
using Tellurium.VisualAssertions.Screenshots.Domain;

namespace Tellurium.VisualAssertions.TestRunnersAdapters.Providers
{
    public class ConsoleTestRunnerAdapter : ITestRunnerAdapter
    {
        private readonly Action<string> testOutputWriter;

        public ConsoleTestRunnerAdapter(Action<string> testOutputWriter)
        {
            this.testOutputWriter = testOutputWriter;
        }

        public void NotifyAboutTestSuccess(string testName, TestSession session, BrowserPattern pattern,
            string testResultMessage)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            testOutputWriter($"Test passed: {testName} {testResultMessage}");
            Console.ResetColor();
        }

        public void NotifyAboutTestFail(string testName, TestSession session, BrowserPattern pattern,
            string testResultMessage)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            testOutputWriter($"Test failed: {testName} {testResultMessage}");
            Console.ResetColor();
        }

        public void NotifyAboutError(Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            testOutputWriter(ex.GetFullExceptionMessage());
            Console.ResetColor();
        }
    }
}