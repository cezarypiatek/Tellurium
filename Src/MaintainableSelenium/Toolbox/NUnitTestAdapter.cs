using System;
using MaintainableSelenium.Toolbox.Screenshots;
using NUnit.Framework;

namespace MaintainableSelenium.Sample.UITests
{
    public class NUnitTestAdapter:ITestRunnerAdapter
    {
        private static readonly string TestSessionId = Guid.NewGuid().ToString();
        private static readonly DateTime StartDate = DateTime.Now;
        
        public TestSessionInfo GetTestSessionInfo()
        {
            return new TestSessionInfo
            {
                SessionId = TestSessionId,
                StartDate = StartDate
            };
        }

        public string GetCurrentTestName()
        {
            return TestContext.CurrentContext.Test.FullName;
        }
    }
}