using System.Collections.Generic;
using MaintainableSelenium.VisualAssertions.Screenshots.Domain;

namespace MaintainableSelenium.VisualAssertions.Web.Models.Home
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