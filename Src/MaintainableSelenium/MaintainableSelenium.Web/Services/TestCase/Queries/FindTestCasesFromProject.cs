using System.Collections.Generic;
using System.Linq;
using MaintainableSelenium.Toolbox.Infrastructure;

namespace MaintainableSelenium.Web.Services.TestCase.Queries
{
    public class FindTestCasesFromProject:IQueryAll<Toolbox.Screenshots.Domain.TestCase>
    {
        private readonly long projectId;

        public FindTestCasesFromProject(long projectId)
        {
            this.projectId = projectId;
        }

        public List<Toolbox.Screenshots.Domain.TestCase> GetQuery(IQueryable<Toolbox.Screenshots.Domain.TestCase> query)
        {
            return query.Where(x => x.Project.Id == projectId).ToList();
        }
    }
}