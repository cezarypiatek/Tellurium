using System.Collections.Generic;
using System.Linq;
using MaintainableSelenium.Toolbox.Infrastructure;

namespace MaintainableSelenium.Web.Services.TestResult.Queries
{
    public class FindFailedTestFromSessionForBrowser:IQueryAll<Toolbox.Screenshots.Domain.TestResult>
    {
        private readonly long testSessionId;
        private readonly string browserName;

        public FindFailedTestFromSessionForBrowser(long testSessionId, string browserName)
        {
            this.testSessionId = testSessionId;
            this.browserName = browserName;
        }

        public List<Toolbox.Screenshots.Domain.TestResult> GetQuery(IQueryable<Toolbox.Screenshots.Domain.TestResult> query)
        {
            return query.Where(x => x.TestSession.Id == testSessionId && x.TestPassed == false && x.BrowserName == browserName)
                    .ToList();
        }
    }
}