using System.Linq;
using Tellurium.VisualAssertion.Dashboard.Services.TestResults;
using Tellurium.VisualAssertion.Dashboard.Services.TestResults.Queries;
using Tellurium.VisualAssertion.Dashboard.Services.WorkSeed;
using Tellurium.VisualAssertions.Infrastructure;
using Tellurium.VisualAssertions.Screenshots.Domain;


namespace Tellurium.VisualAssertion.Dashboard.Services.Queries
{
    public class GetTestSessionFromProjectQuery:IQuery
    {
        public GetTestSessionFromProjectQuery(long projectId)
        {
            this.ProjectId = projectId;
        }

        public long ProjectId { get; set; }
    }
    
    public class GetTestSessionFromProjectQueryHandler:IQueryHandler<GetTestSessionFromProjectQuery,  TestSessionListViewModel>
    {
        private readonly IRepository<TestSession> testSessionRepository;

        public GetTestSessionFromProjectQueryHandler(IRepository<TestSession> testSessionRepository)
        {
            this.testSessionRepository = testSessionRepository;
        }

        public TestSessionListViewModel Execute(GetTestSessionFromProjectQuery query)
        {
            var q = new FindAllSessionFromProject(query.ProjectId);
            var testSessions = this.testSessionRepository.FindAll(q);
            return new TestSessionListViewModel
            {
                TestSessions = testSessions.Select(x=> new TestSessionListItemDTO
                {
                    SessionId = x.Id,
                    StartDate = x.StartDate.ToString("g"),
                    Browsers = x.Browsers.OrderBy(b=>b).ToList()
                }).ToList()
            };
        }
    }
}