using System.Collections.Generic;
using System.Linq;
using MaintainableSelenium.VisualAssertions.Infrastructure;

namespace MaintainableSelenium.VisualAssertions.Web.Services.TestResult.Queries
{
    public class FindFailedTestFromSessionForBrowser:IQueryAll<VisualAssertions.Screenshots.Domain.TestResult>
    {
        private readonly long testSessionId;
        private readonly string browserName;

        public FindFailedTestFromSessionForBrowser(long testSessionId, string browserName)
        {
            this.testSessionId = testSessionId;
            this.browserName = browserName;
        }

        public List<VisualAssertions.Screenshots.Domain.TestResult> GetQuery(IQueryable<VisualAssertions.Screenshots.Domain.TestResult> query)
        {
            return query.Where(x => x.TestSession.Id == testSessionId && x.TestPassed == false && x.BrowserName == browserName)
                    .ToList();
        }
    }
}