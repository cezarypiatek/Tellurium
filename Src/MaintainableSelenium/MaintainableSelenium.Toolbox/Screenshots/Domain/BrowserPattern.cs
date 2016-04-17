using System;
using System.Collections.Generic;
using System.Linq;
using MaintainableSelenium.Toolbox.Infrastructure;

namespace MaintainableSelenium.Toolbox.Screenshots.Domain
{
    public class BrowserPattern:Entity
    {
        public virtual string BrowserName { get; set; }
        public virtual IList<BlindRegion> BlindRegions { get; set; }
        public virtual ScreenshotData PatternScreenshot { get; set; }
        public virtual TestCase TestCase { get; set; }
        public virtual bool IsActive { get; set; }
        public virtual DateTime CreatedOn { get; set; }

        public BrowserPattern()
        {
            BlindRegions = new List<BlindRegion>();
        }

        public virtual bool MatchTo(byte[] screenshot)
        {
            var blindRegions = this.GetAllBlindRegions();
            var screenshotHash = ImageHelpers.ComputeHash(screenshot, blindRegions);
            return screenshotHash == this.PatternScreenshot.Hash;
        }

        public virtual void ReplaceLocalBlindRegionsSet(IList<BlindRegion> newBlindRevionsSet)
        {
            this.BlindRegions.Clear();

            foreach (var localBlindRegion in newBlindRevionsSet)
            {
                this.BlindRegions.Add(localBlindRegion);
            }
            this.UpdateTestCaseHash();
        }

        public virtual IReadOnlyList<BlindRegion> GetAllBlindRegions()
        {
            var result = BlindRegions.ToList();
            var fromProjectLevel = TestCase.Project.GetBlindRegionsForBrowser(BrowserName);
            result.AddRange(fromProjectLevel);
            return result.AsReadOnly();
        }

        public virtual void UpdateTestCaseHash()
        {
            this.PatternScreenshot.Hash = ImageHelpers.ComputeHash(this.PatternScreenshot.Image, this.GetAllBlindRegions());
        }
    }
}