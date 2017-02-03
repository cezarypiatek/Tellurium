using System.Collections.Generic;
using Tellurium.VisualAssertions.Screenshots.Domain;

namespace Tellurium.VisualAssertions.Dashboard.Models.Home
{
    public class SaveLocalBlindRegionsDTO
    {
        public long BrowserPatternId { get; set; }

        public List<BlindRegion> LocalBlindRegions { get; set; }

        public SaveLocalBlindRegionsDTO()
        {
            LocalBlindRegions = new List<BlindRegion>();
        }
    }
}