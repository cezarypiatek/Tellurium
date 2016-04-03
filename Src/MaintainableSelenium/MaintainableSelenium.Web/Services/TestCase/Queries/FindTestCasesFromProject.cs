using System.Collections.Generic;
using System.Linq;
using MaintainableSelenium.Toolbox.Screenshots;

namespace MaintainableSelenium.Web.Services.TestCase.Queries
{
    public class FindTestCasesFromProject:IQueryAll<Toolbox.Screenshots.TestCase>
    {
        private readonly long projectId;

        public FindTestCasesFromProject(long projectId)
        {
            this.projectId = projectId;
        }

        public List<Toolbox.Screenshots.TestCase> GetQuery(IQueryable<Toolbox.Screenshots.TestCase> query)
        {
            return query.Where(x => x.Project.Id == projectId).ToList();
        }
    }
}