using System.Collections.Generic;
using System.Linq;
using MaintainableSelenium.Toolbox.Infrastructure;

namespace MaintainableSelenium.Toolbox.Screenshots.Domain
{
    public class Project:Entity
    {
        public virtual string Name { get; set; }
        public virtual IList<TestSession> Sessions { get; set; }

        public virtual IList<BlindRegionForBrowser> GlobalBlindRegionsForBrowsers { get; set; }
        public virtual IList<TestCase> TestCases { get; set; }


        public Project()
        {
            Sessions = new List<TestSession>(); 
            GlobalBlindRegionsForBrowsers = new List<BlindRegionForBrowser>();
            TestCases = new List<TestCase>();
        }

        public virtual  void AddSession(TestSession session)
        {
            session.Project = this;
            Sessions.Add(session);
        }

        public virtual void AddTestCase(TestCase testCase)
        {
            testCase.Project = this;
            TestCases.Add(testCase);
        }

        public virtual void AddGlobalBlindRegions(BlindRegionForBrowser blindRegionForBrowser)
        {
            this.GlobalBlindRegionsForBrowsers.Add(blindRegionForBrowser);
        }

        public virtual IList<BlindRegion> GetBlindRegionsForBrowser(string browserName)
        {
            var blindRegionsForBrowser = this.GlobalBlindRegionsForBrowsers.FirstOrDefault(x => x.BrowserName == browserName);
            if (blindRegionsForBrowser == null)
            {
                return new List<BlindRegion>();
            }
            return blindRegionsForBrowser.BlindRegions;
        }
    }
}