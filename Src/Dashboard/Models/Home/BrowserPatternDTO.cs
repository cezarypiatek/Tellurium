using System.Collections.Generic;
using Tellurium.VisualAssertions.Screenshots.Domain;

namespace Tellurium.VisualAssertion.Dashboard.Models.Home
{
    public class BrowserPatternDTO
    {
        public long PatternId { get; set; }
        public long TestCaseId { get; set; }
        public List<BlindRegion> GlobalBlindRegions { get; set; }
        public List<BlindRegion> CategoryBlindRegions { get; set; }
        public List<BlindRegion> LocalBlindRegions { get; set; }
        public string BrowserName { get; set; }
    }
}