using System.Collections.Generic;

namespace MaintainableSelenium.Toolbox.Screenshots
{
    public class TestCase:Entity
    {
        public virtual string PatternScreenshotName { get; set; }
        public virtual IList<BrowserPattern> Patterns { get; set; }
        public virtual Project Project { get; set; }

        public virtual void AddPatterns(BrowserPattern browserPattern)
        {
            browserPattern.TestCase = this;
            Patterns.Add(browserPattern);
        }
        
        public virtual void AddNewPattern(byte[] screenshot, string browserName )
        {
            var globalBlindRegions = this.Project.GetBlindRegionsForBrowser(browserName);
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
}