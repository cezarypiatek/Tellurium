using System;
using System.Collections.Generic;

namespace MaintainableSelenium.Toolbox.Screenshots
{
    public class TestSessionInfo
    {
        public string SessionId { get; set; }
        public DateTime StartDate { get; set; }
    }

    public class ExtendedTestSessionInfo
    {
        public TestSessionInfo TestSession { get; set; }
        public List<string> Browsers { get; set; }
    }

    public class TestResultInfo
    {
        public string Id { get; set; }
        public string TestCaseId { get; set; }
        public TestSessionInfo TestSession { get; set; }
        public string TestName { get; set; }
        public string ScreenshotName { get; set; }
        public string BrowserName { get; set; }
        public bool TestPassed { get; set; }
        public ScreenshotData ErrorScreenshot { get; set; }
    }
    
    public class TestCaseInfo
    {
        public string Id { get; set; }
        public string TestName { get; set; }
        public string PatternScreenshotName { get; set; }
        public string BrowserName { get; set; }
        public string PatternScreenhotHash { get; set; }
        public List<BlindRegion> BlindRegions { get; set; }
        public byte[] PatternScreenshot { get; set; }

        public TestCaseInfo()
        {
            BlindRegions = new List<BlindRegion>();
        }
    }

    public class ExtendedTestCaseInfo
    {
        public string TestCaseName { get; set; }
        public List<TestCaseShortcut> Browsers { get; set; }
    }

    public class TestCaseShortcut
    {
        public string BrowserName { get; set; }
        public string TestCaseId { get; set; }
    }

    public class ScreenshotData
    {
        public string Hash { get; set; }
        public byte[] Image { get; set; }
    }
}