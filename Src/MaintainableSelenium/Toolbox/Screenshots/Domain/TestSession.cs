using System;
using System.Collections.Generic;

namespace MaintainableSelenium.Toolbox.Screenshots
{
    public class TestSession:Entity
    {
        public DateTime StartDate { get; set; }
        public List<TestResult> TestResults { get; set; }
        public ISet<string> Browsers { get; set; }

        public void AddTestResult(TestResult testResult)
        {
            Browsers.Add(testResult.BrowserName);
            TestResults.Add(testResult);
            testResult.TestSession = this;
        }
    }
}