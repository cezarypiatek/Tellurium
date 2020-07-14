using System;
using System.Collections.Generic;
using System.Linq;
using Tellurium.VisualAssertions.Infrastructure;

namespace Tellurium.VisualAssertions.Screenshots.Domain
{
    public class BrowserPattern : Entity
    {
        public virtual string BrowserName { get; set; }
        public virtual IList<BlindRegion> BlindRegions { get; set; } = new List<BlindRegion>();
        public virtual ScreenshotData PatternScreenshot { get; set; }
        public virtual TestCase TestCase { get; set; }
        public virtual bool IsActive { get; set; }
        public virtual DateTime CreatedOn { get; set; }

        public virtual void ReplaceLocalBlindRegionsSet(IList<BlindRegion> newBlindRegionsSet)
        {
            this.BlindRegions.Clear();

            foreach (var localBlindRegion in newBlindRegionsSet)
            {
                this.BlindRegions.Add(localBlindRegion);
            }
            this.UpdateTestCaseHash();
        }

        protected virtual void UpdateTestCaseHash()
        {
            var blindRegions = this.GetAllBlindRegions();
            this.PatternScreenshot.UpdateHash(blindRegions);
        }

        public virtual IReadOnlyList<BlindRegion> GetAllBlindRegions()
        {
            var result = BlindRegions.ToList();
            var fromAboveLevels = TestCase.Category.GetAllBlindRegionsForBrowser(BrowserName);
            result.AddRange(fromAboveLevels);
            return result.AsReadOnly();
        }

        public virtual void Deactivate()
        {
            this.IsActive = false;
        }

        public virtual IList<BlindRegion> GetCopyOfOwnBlindRegions()
        {
            return this.BlindRegions.Select(x => new BlindRegion
            {
                Left = x.Left,
                Top = x.Top,
                Width = x.Width,
                Height = x.Height
            }).ToList();
        } 
        
        public virtual IList<BlindRegion> GetCopyOfAllBlindRegions()
        {
            return this.GetAllBlindRegions().Select(x => new BlindRegion
            {
                Left = x.Left,
                Top = x.Top,
                Width = x.Width,
                Height = x.Height
            }).ToList();
        }
    }
}