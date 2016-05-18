using System;
using System.Collections.Generic;
using System.Linq;
using MaintainableSelenium.VisualAssertions.Infrastructure;

namespace MaintainableSelenium.VisualAssertions.Screenshots.Domain
{
    public class TestCase:Entity
    {
        public virtual string PatternScreenshotName { get; set; }
        public virtual TestCaseCategory Category { get; set; }
        public virtual IList<BrowserPattern> Patterns { get; set; }
        public virtual Project Project { get; set; }

        public virtual void AddNewPattern(byte[] screenshot, string browserName)
        {
            var blindRegions = this.Category.GetAllBlindRegionsForBrowser(browserName);
            var newPattern = new BrowserPattern
            {
                TestCase = this,
                BrowserName = browserName,
                PatternScreenshot = new ScreenshotData
                {
                    Image = screenshot,
                    Hash = ImageHelpers.ComputeHash(screenshot, blindRegions)
                },
                IsActive = true,
                CreatedOn = DateTime.Now
            };
            this.Patterns.Add(newPattern);
        }

        public virtual BrowserPattern GetActivePatternForBrowser(string browserName)
        {
            return Patterns.SingleOrDefault(x => x.BrowserName == browserName && x.IsActive);
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