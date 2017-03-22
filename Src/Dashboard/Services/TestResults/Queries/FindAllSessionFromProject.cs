using System.Collections.Generic;
using System.Linq;
using Tellurium.VisualAssertions.Infrastructure;
using Tellurium.VisualAssertions.Screenshots.Domain;

namespace Tellurium.VisualAssertion.Dashboard.Services.TestResults.Queries
{
    public class FindAllSessionFromProject:IQueryAll<TestSession>
    {
        private readonly long projectId;

        public FindAllSessionFromProject(long projectId)
        {
            this.projectId = projectId;
        }

        public List<TestSession> GetQuery(IQueryable<TestSession> query)
        {
            return query.Where(x => x.Project.Id == projectId)
                .OrderByDescending(x => x.StartDate).ToList();
        }
    }
}