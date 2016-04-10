using System;
using System.Collections.Generic;
using System.Linq;
using MaintainableSelenium.Toolbox.Infrastructure;

namespace MaintainableSelenium.Toolbox.Screenshots.Domain
{
    public class TestCase:Entity
    {
        public virtual string PatternScreenshotName { get; set; }
        public virtual string Category { get; set; }
        public virtual IList<BrowserPattern> Patterns { get; set; }
        public virtual Project Project { get; set; }

        public virtual void AddNewPattern(byte[] screenshot, string browserName )
        {
            var globalBlindRegions = this.Project.GetBlindRegionsForBrowser(browserName);
            var browserPattern = new BrowserPattern
            {
                TestCase = this,
                BrowserName = browserName,
                PatternScreenshot = new ScreenshotData
                {
                    Image = screenshot,
                    Hash = ImageHelpers.ComputeHash(screenshot, globalBlindRegions)
                },
                IsActive = true,
                CreatedOn = DateTime.Now
            };
            this.Patterns.Add(browserPattern);
        }

        public virtual void UpdatePatternScreenshot(BrowserPattern pattern, byte[] newScreenshot)
        {
            pattern.IsActive = false;
            this.AddNewPattern(newScreenshot, pattern.BrowserName);
        }

        public virtual BrowserPattern GetPatternForBrowser(string browserName)
        {
            return Patterns.FirstOrDefault(x => x.BrowserName == browserName);
        }

        public virtual List<BrowserPattern> GetActivePatterns()
        {
            return this.Patterns.Where(x=>x.IsActive).ToList();
        }

        public TestCase()
        {
            Patterns = new List<BrowserPattern>();
        }
    }
}