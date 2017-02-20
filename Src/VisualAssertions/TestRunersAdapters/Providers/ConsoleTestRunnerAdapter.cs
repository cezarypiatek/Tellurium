using System;
using Tellurium.MvcPages.Utils;
using Tellurium.VisualAssertions.Screenshots.Domain;

namespace Tellurium.VisualAssertions.TestRunersAdapters.Providers
{
    public class ConsoleTestRunnerAdapter : ITestRunnerAdapter
    {
        private readonly Action<string> testOuputWriter;

        public ConsoleTestRunnerAdapter(Action<string> testOuputWriter)
        {
            this.testOuputWriter = testOuputWriter;
        }

        public void NotifyAboutTestSuccess(string testName, TestSession session, BrowserPattern pattern)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            testOuputWriter($"Test passed: {testName}");
            Console.ResetColor();
        }

        public void NotifyAboutTestFail(string testName, TestSession session, BrowserPattern pattern)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            testOuputWriter($"Test failed: {testName}");
            Console.ResetColor();
        }

        public void NotifyAboutError(Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            testOuputWriter(ex.GetFullExceptionMessage());
            Console.ResetColor();
        }
    }
}