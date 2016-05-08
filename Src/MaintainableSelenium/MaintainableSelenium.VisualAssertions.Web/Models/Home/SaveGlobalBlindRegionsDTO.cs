using System.Collections.Generic;
using MaintainableSelenium.VisualAssertions.Screenshots.Domain;

namespace MaintainableSelenium.VisualAssertions.Web.Models.Home
{
    public class SaveGlobalBlindRegionsDTO
    {
        public string BrowserName { get; set; }
        public List<BlindRegion> BlindRegions { get; set; }
        public long TestCaseId { get; set; }

        public SaveGlobalBlindRegionsDTO()
        {
            BlindRegions = new List<BlindRegion>();
        }
    }
}