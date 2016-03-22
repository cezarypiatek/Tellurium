using System.Collections.Generic;

namespace MaintainableSelenium.Toolbox.Screenshots
{
    public class TestCaseSet
    {
        public string Id { get; set; }
        public string Browser { get; set; }
        public List<BlindRegion> GlobalBlindRegions { get; set; }
        public List<TestCase> TestCases { get; set; }
    }
}