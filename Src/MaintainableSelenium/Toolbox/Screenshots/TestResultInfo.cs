using System.Collections.Generic;

namespace MaintainableSelenium.Toolbox.Screenshots
{
    public class TestResultInfo
    {
        public string TestSessionId { get; set; }
        public string TestName { get; set; }
        public string ScreenshotName { get; set; }
        public string BrowserName { get; set; }
        public bool TestPassed { get; set; }
        public ScreenshotData ErrorScreenshot { get; set; }
    }

    public class TestCaseInfo
    {
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

    public class ScreenshotData
    {
        public string Hash { get; set; }
        public byte[] Image { get; set; }
    }
}