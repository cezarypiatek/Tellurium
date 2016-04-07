using System.Collections.Generic;
using System.Linq;
using MaintainableSelenium.Toolbox.Infrastructure;
using MaintainableSelenium.Toolbox.Screenshots.Domain;

namespace MaintainableSelenium.Web.Services.TestResult.Queries
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