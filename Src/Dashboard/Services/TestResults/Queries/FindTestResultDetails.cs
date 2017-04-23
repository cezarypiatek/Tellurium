using System.Linq;
using NHibernate.Linq;
using Tellurium.VisualAssertions.Infrastructure;
using Tellurium.VisualAssertions.Screenshots.Domain;

namespace Tellurium.VisualAssertion.Dashboard.Services.TestResults.Queries
{
    public class FindTestResultDetails: IQueryOne<TestResult>
    {
        private readonly long testResultId;

        public FindTestResultDetails(long testResultId)
        {
            this.testResultId = testResultId;
        }

        public TestResult GetQuery(IQueryable<TestResult> query)
        {
            return query.Where(x => x.Id == testResultId).Fetch(x => x.Pattern).First();
        }
    }
}