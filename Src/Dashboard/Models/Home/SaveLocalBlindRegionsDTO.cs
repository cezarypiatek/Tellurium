using System.Collections.Generic;
using Tellurium.VisualAssertions.Screenshots.Domain;

namespace Tellurium.VisualAssertion.Dashboard.Models.Home
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


    public class SaveBlindRegionsDTO
    {
        public SaveLocalBlindRegionsDTO Local { get; set; }
        public SaveCategoryBlindRegionsDTO Category { get; set; }
        public SaveGlobalBlindRegionsDTO Global { get; set; }
    }
}