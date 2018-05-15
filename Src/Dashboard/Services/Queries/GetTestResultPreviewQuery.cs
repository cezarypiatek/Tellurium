using Tellurium.VisualAssertion.Dashboard.Services.TestResults;
using Tellurium.VisualAssertion.Dashboard.Services.TestResults.Queries;
using Tellurium.VisualAssertion.Dashboard.Services.WorkSeed;
using Tellurium.VisualAssertions.Infrastructure;
using Tellurium.VisualAssertions.Screenshots.Domain;

namespace Tellurium.VisualAssertion.Dashboard.Services.Queries
{
    public class GetTestResultPreviewQuery : IQuery
    {
        public long TestSessionId { get; set; }
        public long PatternId { get; set; }
    }

    public class GetTestResultPreviewQueryViewModel
    {
        public TestResultListItemDTO ListItem { get; set; }
        public TestResultDetailsViewModel Preview { get; set; }
    }

    public class GetTestResultPreviewQueryHandler : IQueryHandler<GetTestResultPreviewQuery, GetTestResultPreviewQueryViewModel>
    {
        private readonly IRepository<TestResult> testRepository;

        public GetTestResultPreviewQueryHandler(IRepository<TestResult> testRepository)
        {
            this.testRepository = testRepository;
        }

        public GetTestResultPreviewQueryViewModel Execute(GetTestResultPreviewQuery query)
        {
            var q = new FindTestResultForPatternInSession(query.TestSessionId, query.PatternId);
            var testResult = this.testRepository.FindOne(q);
            return new GetTestResultPreviewQueryViewModel()
            {
                ListItem = MapToTestResultListItemDTO(testResult),
                Preview = this.GetTestResultDetails(testResult.Id)
            };
        }
    }
}