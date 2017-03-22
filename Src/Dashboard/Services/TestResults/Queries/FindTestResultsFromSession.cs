using System;
using System.Collections.Generic;
using System.Linq;
using Tellurium.VisualAssertions.Infrastructure;
using Tellurium.VisualAssertions.Screenshots.Domain;

namespace Tellurium.VisualAssertion.Dashboard.Services.TestResults.Queries
{
    public class FindTestResultsFromSession : IQueryAll<TestResult>
    {
        private readonly long sessionId;
        private readonly string browserName;
        private readonly TestResultStatus resultStatus;

        public FindTestResultsFromSession(long sessionId, string browserName, TestResultStatus resultStatus)
        {
            this.sessionId = sessionId;
            this.browserName = browserName;
            this.resultStatus = resultStatus;
        }

        public List<TestResult> GetQuery(IQueryable<TestResult> query)
        {
            var testFromSession = query.Where(x=>x.TestSession.Id == sessionId)
                .Where(x=>x.BrowserName == browserName);

            switch (resultStatus)
            {
                case TestResultStatus.All:
                    return testFromSession.ToList();
                case TestResultStatus.Passed:
                    return testFromSession.Where(x=>x.TestPassed).ToList();
                case TestResultStatus.Failed:
                    return testFromSession.Where(x => x.TestPassed == false).ToList();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}