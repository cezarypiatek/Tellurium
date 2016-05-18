using System.Collections.Generic;
using System.Linq;
using MaintainableSelenium.VisualAssertions.Infrastructure;
using MaintainableSelenium.VisualAssertions.Screenshots.Domain;

namespace MaintainableSelenium.VisualAssertions.Web.Services.TestResults.Queries
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