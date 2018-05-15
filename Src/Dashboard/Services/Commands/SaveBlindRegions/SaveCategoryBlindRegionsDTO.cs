using System.Collections.Generic;
using Tellurium.VisualAssertions.Screenshots.Domain;

namespace Tellurium.VisualAssertion.Dashboard.Services.Commands.SaveBlindRegions
{
    public class SaveCategoryBlindRegionsDTO
    {
        public string BrowserName { get; set; }
        public List<BlindRegion> BlindRegions { get; set; }
        public long TestCaseId { get; set; }

        public SaveCategoryBlindRegionsDTO()
        {
            BlindRegions = new List<BlindRegion>();
        }
    }
}