using System.Collections.Generic;
using Tellurium.VisualAssertion.Dashboard.Services.TestResults;
using Tellurium.VisualAssertion.Dashboard.Services.TestResults.Queries;
using Tellurium.VisualAssertion.Dashboard.Services.WorkSeed;
using Tellurium.VisualAssertions.Infrastructure;
using Tellurium.VisualAssertions.Screenshots.Domain;
using System.Linq;

namespace Tellurium.VisualAssertion.Dashboard.Services.Queries
{
    public enum TestResultStatusFilter
    {
        All,
        Passed,
        Failed,
        New
    }

    public class GetTestsFromSessionQuery : IQuery
    {
        public long SessionId { get; set; }
        public string BrowserName { get; set; }
        public TestResultStatusFilter TestStatus { get; set; }
    }

    public class GetTestsFromSessionViewModel
    {
        public List<TestResultListItemDTO> TestResults { get; set; }
        public long TestSessionId { get; set; }
        public string BrowserName { get; set; }
        public int AllCount { get; set; }
        public int PassedCount { get; set; }
        public int FailedCount { get; set; }
        public int NewCount { get; set; }
    }

    public class GetTestsFromSessionQueryHandler : IQueryHandler<GetTestsFromSessionQuery, GetTestsFromSessionViewModel>
    {
        private readonly IRepository<TestResult> testRepository;

        public GetTestsFromSessionQueryHandler(IRepository<TestResult> testRepository)
        {
            this.testRepository = testRepository;
        }

        public GetTestsFromSessionViewModel Execute(GetTestsFromSessionQuery query)
        {
            var q = new FindTestResultsFromSession(query.SessionId, query.BrowserName, query.TestStatus);
            var testResults = this.testRepository.FindAll(q);
            var failedCount = testResults.Count(x => x.Status == TestResultStatus.Failed);
            var passedCount = testResults.Count(x => x.Status == TestResultStatus.Passed);
            var newCount = testResults.Count(x => x.Status == TestResultStatus.NewPattern);
            return new GetTestsFromSessionViewModel()
            {
                AllCount = testResults.Count,
                FailedCount = failedCount,
                PassedCount = passedCount,
                NewCount = newCount,
                TestSessionId = query.SessionId,
                BrowserName = query.BrowserName,
                TestResults = testResults.ConvertAll(MapToTestResultListItemDTO)
            };
        }

        private static TestResultListItemDTO MapToTestResultListItemDTO(TestResult x)
        {
            return new TestResultListItemDTO()
            {
                TestResultId = x.Id,
                TestCaseId = x.Pattern.TestCase.Id,
                TestPatternId = x.Pattern.Id,
                TestPassed = x.Status==TestResultStatus.Passed,
                TestFailed = x.Status==TestResultStatus.Failed,
                ScreenshotName = $"{x.Category} \\ {x.ScreenshotName}",
                CanShowMarkAsPattern = x.Status == TestResultStatus.Failed && x.Pattern.IsActive,
            };
        }
    }
}