using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MaintainableSelenium.Toolbox.Infrastructure;

namespace MaintainableSelenium.Toolbox.Screenshots.Domain
{
    public class Project:Entity, IBlindRegionForBrowserOwner
    {
        public virtual string Name { get; set; }
        public virtual IList<TestSession> Sessions { get; set; }
        public virtual IList<BlindRegionForBrowser> GlobalBlindRegionsForBrowsers { get; set; }
        public virtual IList<TestCaseCategory> TestCaseCategories { get; set; }


        public Project()
        {
            Sessions = new List<TestSession>(); 
            GlobalBlindRegionsForBrowsers = new List<BlindRegionForBrowser>();
            TestCaseCategories = new List<TestCaseCategory>();
        }

        public virtual  void AddSession(TestSession session)
        {
            session.Project = this;
            Sessions.Add(session);
        }

        public virtual TestCaseCategory AddTestCaseCategory(string categoryName)
        {
            var newCategory = new TestCaseCategory()
            {
                Name = categoryName,
                Project = this
            };
            TestCaseCategories.Add(newCategory);
            return newCategory;
        }

        public virtual void AddBlindRegionForBrowser(BlindRegionForBrowser blindRegionForBrowser)
        {
            this.GlobalBlindRegionsForBrowsers.Add(blindRegionForBrowser);
        }

        public virtual BlindRegionForBrowser GetOwnBlindRegionForBrowser(string browserName)
        {
            return this.GlobalBlindRegionsForBrowsers.FirstOrDefault(x => x.BrowserName == browserName);
        }

        public virtual IReadOnlyList<BlindRegion> GetBlindRegionsForBrowser(string browserName)
        {
            var blindRegionsForBrowser = this.GlobalBlindRegionsForBrowsers.FirstOrDefault(x => x.BrowserName == browserName);
            if (blindRegionsForBrowser == null)
            {
                return new List<BlindRegion>();
            }
            return new ReadOnlyCollection<BlindRegion>(blindRegionsForBrowser.BlindRegions);
        }
    }
}