using System;
using System.Collections.Generic;
using Tellurium.VisualAssertions.Infrastructure;

namespace Tellurium.VisualAssertions.Screenshots.Domain
{
    public class TestSession:Entity
    {
        public virtual DateTime StartDate { get; set; }
        public virtual IList<TestResult> TestResults { get; set; }
        public virtual ISet<string> Browsers { get; set; }
        public virtual Project Project { get; set; }

        public TestSession()
        {
            Browsers = new HashSet<string>();
            TestResults = new List<TestResult>();
        }

        public virtual void AddTestResult(TestResult testResult)
        {
            Browsers.Add(testResult.BrowserName);
            TestResults.Add(testResult);
            testResult.TestSession = this;
        }
    }
}