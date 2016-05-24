using System.Linq;
using MaintainableSelenium.VisualAssertions.Infrastructure;
using MaintainableSelenium.VisualAssertions.Screenshots.Domain;

namespace MaintainableSelenium.VisualAssertions.Web.Services.TestResults.Queries
{
    public class FindTestResultForPatternInSession:IQueryOne<TestResult>
    {
        private readonly long testSessionId;
        private readonly long patternId;

        public FindTestResultForPatternInSession(long testSessionId, long patternId)
        {
            this.testSessionId = testSessionId;
            this.patternId = patternId;
        }

        public TestResult GetQuery(IQueryable<TestResult> query)
        {
            return query.Single(x => x.TestSession.Id == testSessionId && x.Pattern.Id == patternId);
        }
    }
}