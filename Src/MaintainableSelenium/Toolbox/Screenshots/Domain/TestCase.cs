using System.Collections.Generic;

namespace MaintainableSelenium.Toolbox.Screenshots
{
    public class TestCase:Entity
    {
        public string TestName { get; set; }
        public string PatternScreenshotName { get; set; }
        public List<BrowserPattern> Patterns { get; set; }
        public TestCaseSet TestCaseSet { get; set; }

        public void AddPatterns(BrowserPattern browserPattern)
        {
            browserPattern.TestCase = this;
            Patterns.Add(browserPattern);
        }
        
        public void AddNewPattern(byte[] screenshot, string browserName )
        {
            var globalBlindRegions = this.TestCaseSet.GetBlindRegionsForBrowser(browserName);
            var browserPattern = new BrowserPattern
            {
                BrowserName = browserName,
                PatternScreenshot = new ScreenshotData
                {
                    Image = screenshot,
                    Hash = ImageHelpers.ComputeHash(screenshot, globalBlindRegions)
                }
            };
            this.AddPatterns(browserPattern);
        }

        public TestCase()
        {
            Patterns = new List<BrowserPattern>();
        }
    }

    public class BrowserPattern:Entity
    {
        public string BrowserName { get; set; }
        public List<BlindRegion> BlindRegions { get; set; }
        public ScreenshotData PatternScreenshot { get; set; }
        public TestCase TestCase { get; set; }

        public BrowserPattern()
        {
            BlindRegions = new List<BlindRegion>();
        }

        public bool MatchTo(byte[] screenshot)
        {
            var globalBlindRegions = TestCase.TestCaseSet.GetBlindRegionsForBrowser(BrowserName);
            var screenshotHash = ImageHelpers.ComputeHash(screenshot, globalBlindRegions, this.BlindRegions);
            return screenshotHash == this.PatternScreenshot.Hash;
        }
    }
}