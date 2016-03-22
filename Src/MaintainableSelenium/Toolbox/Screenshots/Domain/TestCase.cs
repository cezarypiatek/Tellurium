using System.Collections.Generic;

namespace MaintainableSelenium.Toolbox.Screenshots
{
    public class TestCase:Entity
    {
        public string TestName { get; set; }
        public string PatternScreenshotName { get; set; }
        public string BrowserName { get; set; }
        public List<BlindRegion> BlindRegions { get; set; }
        public ScreenshotData PatternScreenshot { get; set; }

        public TestCase()
        {
            BlindRegions = new List<BlindRegion>();
        }
    }
}