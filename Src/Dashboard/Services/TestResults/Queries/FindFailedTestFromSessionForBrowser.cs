using System.Collections.Generic;
using System.Linq;
using Tellurium.VisualAssertions.Infrastructure;
using Tellurium.VisualAssertions.Screenshots.Domain;

namespace Tellurium.VisualAssertion.Dashboard.Services.TestResults.Queries
{
    public class FindFailedTestFromSessionForBrowser:IQueryAll<TestResult>
    {
        private readonly long testSessionId;
        private readonly string browserName;

        public FindFailedTestFromSessionForBrowser(long testSessionId, string browserName)
        {
            this.testSessionId = testSessionId;
            this.browserName = browserName;
        }

        public List<TestResult> GetQuery(IQueryable<TestResult> query)
        {
            return query.Where(x => x.TestSession.Id == testSessionId && x.TestPassed == false && x.BrowserName == browserName)
                    .ToList();
        }
    }
}