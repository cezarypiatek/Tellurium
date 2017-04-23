using System.Linq;
using NHibernate.Linq;
using Tellurium.VisualAssertions.Infrastructure;
using Tellurium.VisualAssertions.Screenshots.Domain;

namespace Tellurium.VisualAssertions.Screenshots.Queries
{
    public class FindtActivePatternForBrowser:IQueryOne<BrowserPattern>
    {
        private readonly long testCaseId;
        private readonly string browserName;

        public FindtActivePatternForBrowser(long testCaseId, string browserName)
        {
            this.testCaseId = testCaseId;
            this.browserName = browserName;
        }

        public BrowserPattern GetQuery(IQueryable<BrowserPattern> query)
        {
            return query.Where(x => x.TestCase.Id == testCaseId)
                .Where(x => x.BrowserName == browserName)
                .Where(x => x.IsActive)
                .Fetch(x => x.PatternScreenshot)
                .FetchMany(x => x.BlindRegions)
                .FirstOrDefault();
        }
    }
}