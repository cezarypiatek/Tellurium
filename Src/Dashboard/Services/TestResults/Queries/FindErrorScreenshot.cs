using System.Linq;
using NHibernate.Linq;
using Tellurium.VisualAssertions.Infrastructure;
using Tellurium.VisualAssertions.Screenshots.Domain;

namespace Tellurium.VisualAssertion.Dashboard.Services.TestResults.Queries
{
    public class FindErrorScreenshot:IQueryOne<TestResult>
    {
        private readonly long testId;

        public FindErrorScreenshot(long testId)
        {
            this.testId = testId;
        }

        public TestResult GetQuery(IQueryable<TestResult> query)
        {
            return query.Where(x => x.Id == testId)
                .Fetch(x => x.Pattern)
                .ThenFetch(x=>x.PatternScreenshot)
                .First();
        }
    }
}