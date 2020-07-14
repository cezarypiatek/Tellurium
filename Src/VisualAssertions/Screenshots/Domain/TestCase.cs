using System;
using System.Collections.Generic;
using System.Linq;
using Tellurium.VisualAssertions.Infrastructure;

namespace Tellurium.VisualAssertions.Screenshots.Domain
{
    public class TestCase : Entity
    {
        public virtual string PatternScreenshotName { get; set; }
        public virtual TestCaseCategory Category { get; set; }
        public virtual IList<BrowserPattern> Patterns { get; set; }
        public virtual Project Project { get; set; }

        public virtual BrowserPattern AddNewPattern(byte[] screenshot, string browserName, IList<BlindRegion> blindRegions =null)
        {
            var ownBlindRegions = blindRegions ?? new List<BlindRegion>();
            var inheritedBlindRegions = this.Category.GetAllBlindRegionsForBrowser(browserName);
            var allBlindRegions = inheritedBlindRegions.Concat(ownBlindRegions).ToList();
            var newPattern = new BrowserPattern
            {
                BrowserName = browserName,
                PatternScreenshot = new ScreenshotData
                {
                    Image = screenshot,
                    Hash = ImageHelpers.ComputeHash(screenshot, allBlindRegions)
                },
                IsActive = true,
                CreatedOn = DateTime.Now,
                BlindRegions = ownBlindRegions,
                TestCase = this
            };
            this.Patterns.Add(newPattern);
            return newPattern;
        }

        public virtual List<BrowserPattern> GetActivePatterns()
        {
            return this.Patterns.Where(x=>x.IsActive).ToList();
        }

        public TestCase()
        {
            Patterns = new List<BrowserPattern>();
        }

        public virtual BrowserPattern GetActivePatternForBrowser(string browserName)
        {
            var activePatterns = Patterns.Where(x => x.BrowserName == browserName && x.IsActive).ToList();
            if (activePatterns.Count > 1)
            {
                throw new DuplicatedActivePattern(Project.Name, Category.Name, PatternScreenshotName, browserName);
            }
            return activePatterns.FirstOrDefault();
        }
    }

    public class DuplicatedActivePattern:ApplicationException
    {
        public DuplicatedActivePattern(string project, string category, string test, string browser) 
            : base($"Duplicated active patter {project} / {category} / {test} for browser {browser}")
        {
        }
    }
}