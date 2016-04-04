using System.Collections.Generic;
using System.Linq;
using MaintainableSelenium.Toolbox.Infrastructure;
using MaintainableSelenium.Toolbox.Screenshots.Domain;
using NHibernate.Linq;

namespace MaintainableSelenium.Toolbox.Screenshots.Queries
{
    public class FindPatternsForBrowserInProject:IQueryAll<BrowserPattern>
    {
        private readonly long projectId;
        private readonly string browserName;

        public FindPatternsForBrowserInProject(long projectId, string browserName)
        {
            this.projectId = projectId;
            this.browserName = browserName;
        }

        public List<BrowserPattern> GetQuery(IQueryable<BrowserPattern> query)
        {
            return query.Where(x => x.BrowserName == browserName && x.TestCase.Project.Id == projectId)
                .Fetch(x=>x.BlindRegions)
                .Fetch(x=>x.PatternScreenshot)
                .ToList();
        }
    }
}