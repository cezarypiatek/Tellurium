using System;
using MaintainableSelenium.Toolbox.Screenshots;
using NUnit.Framework;

namespace MaintainableSelenium.Sample.UITests
{
    public class NUnitTestAdapter:ITestRunnerAdapter
    {
        private static readonly string TestSessionId = Guid.NewGuid().ToString();

        public string GetTestSessionId()
        {
            return TestSessionId;
        }

        public string GetCurrentTestName()
        {
            return TestContext.CurrentContext.Test.FullName;
        }
    }
}