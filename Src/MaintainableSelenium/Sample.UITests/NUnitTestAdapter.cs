using System;
using MaintainableSelenium.Toolbox.Screenshots;
using NUnit.Framework;

namespace MaintainableSelenium.Sample.UITests
{
    public class NUnitTestAdapter:ITestRunnerAdapter
    {
        private static readonly DateTime StartDate = DateTime.Now;
        
        public TestSession GetTestSessionInfo()
        {
            return new TestSession
            {
                StartDate = StartDate
            };
        }

        public string GetCurrentTestName()
        {
            return TestContext.CurrentContext.Test.FullName;
        }
    }
}