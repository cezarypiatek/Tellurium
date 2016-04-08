using System;
using System.Collections.Generic;
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
            var globalBlindRegions = TestCase.Project.GetBlindRegionsForBrowser(BrowserName);
            var screenshotHash = ImageHelpers.ComputeHash(screenshot, globalBlindRegions, this.BlindRegions);
            return screenshotHash == this.PatternScreenshot.Hash;
        }

        public virtual void ReplaceBlindregionsSet(IList<BlindRegion> newBlindRevionsSet)
        {
            this.BlindRegions.Clear();

            foreach (var localBlindRegion in newBlindRevionsSet)
            {
                this.BlindRegions.Add(localBlindRegion);
            }
        }
    }
}