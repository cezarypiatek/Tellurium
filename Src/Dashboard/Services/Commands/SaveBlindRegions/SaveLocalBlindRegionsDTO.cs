using System.Collections.Generic;
using Tellurium.VisualAssertions.Screenshots.Domain;

namespace Tellurium.VisualAssertion.Dashboard.Services.Commands.SaveBlindRegion
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