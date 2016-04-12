using System.Collections.Generic;
using System.Linq;
using MaintainableSelenium.Toolbox.Infrastructure;
using MaintainableSelenium.Toolbox.Screenshots.Domain;

namespace MaintainableSelenium.Web.Services.TestCase.Queries
{
    public class FindTestCaseCategoriesFromProject:IQueryAll<TestCaseCategory>
    {
        private readonly long projectId;

        public FindTestCaseCategoriesFromProject(long projectId)
        {
            this.projectId = projectId;
        }

        public List<TestCaseCategory> GetQuery(IQueryable<TestCaseCategory> query)
        {
            return query.Where(x => x.Project.Id == this.projectId).ToList();
        }
    }
}