using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using Tellurium.VisualAssertions.Infrastructure;
using Tellurium.VisualAssertions.Screenshots.Domain;

namespace Tellurium.VisualAssertion.Dashboard.Services.TestResults.Queries
{
    public class FindTestResultsFromSession : IQueryAll<TestResult>
    {
        private readonly long sessionId;
        private readonly string browserName;
        private readonly TestResultStatusFilter resultStatus;

        public FindTestResultsFromSession(long sessionId, string browserName, TestResultStatusFilter resultStatus)
        {
            this.sessionId = sessionId;
            this.browserName = browserName;
            this.resultStatus = resultStatus;
        }

        public List<TestResult> GetQuery(IQueryable<TestResult> query)
        {
            var testFromSession = query.Where(x=>x.TestSession.Id == sessionId)
                .Fetch(x=>x.Pattern)
                .Where(x=>x.BrowserName == browserName);

            switch (resultStatus)
            {
                case TestResultStatusFilter.All:
                    return testFromSession.ToList();
                case TestResultStatusFilter.Passed:
                    return testFromSession.Where(x=>x.Status == TestResultStatus.Passed).ToList();
                case TestResultStatusFilter.Failed:
                    return testFromSession.Where(x => x.Status == TestResultStatus.Failed).ToList();
                case TestResultStatusFilter.New:
                    return testFromSession.Where(x => x.Status == TestResultStatus.NewPattern).ToList();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}