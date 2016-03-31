using System.Collections.Generic;
using System.Linq;

namespace MaintainableSelenium.Toolbox.Screenshots
{
    public class TestCaseSet: Entity
    {
        public List<BlindRegionForBrowser> GlobalBlindRegions { get; set; }
        
        public List<TestCase> TestCases { get; set; }

        public TestCaseSet()
        {
            GlobalBlindRegions = new List<BlindRegionForBrowser>();
            TestCases = new List<TestCase>();
        }

        public void AddTestCase(TestCase testCase)
        {
            testCase.TestCaseSet = this;
            TestCases.Add(testCase);
        }

        public List<BlindRegion> GetBlindRegionsForBrowser(string browserName)
        {
            var blindRegionsForBrowser = this.GlobalBlindRegions.FirstOrDefault(x => x.BrowserName == browserName);
            if (blindRegionsForBrowser == null)
            {
                return new List<BlindRegion>();
            }
            return blindRegionsForBrowser.BlindRegions;
        }
    }

    public class BlindRegionForBrowser:Entity
    {
        public List<BlindRegion> BlindRegions { get; set; }
        public string BrowserName { get; set; }

        public BlindRegionForBrowser()
        {
            BlindRegions = new List<BlindRegion>();
        }
    }
}