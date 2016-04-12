using System.Collections.Generic;
using MaintainableSelenium.Toolbox.Infrastructure;

namespace MaintainableSelenium.Toolbox.Screenshots.Domain
{
    public class BlindRegionForBrowser:Entity
    {
        public virtual IList<BlindRegion> BlindRegions { get; set; }
        public virtual string BrowserName { get; set; }

        public BlindRegionForBrowser()
        {
            BlindRegions = new List<BlindRegion>();
        }

        public virtual void ReplaceBlindRegionsSet(IList<BlindRegion> newBlindRegionsSet)
        {
            this.BlindRegions.Clear();
            foreach (var blindRegion in newBlindRegionsSet)
            {
                this.BlindRegions.Add(blindRegion);
            }
        }
    }
}