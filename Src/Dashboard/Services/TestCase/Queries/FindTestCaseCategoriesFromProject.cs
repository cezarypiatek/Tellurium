using System.Collections.Generic;
using System.Linq;
using Tellurium.VisualAssertions.Infrastructure;
using Tellurium.VisualAssertions.Screenshots.Domain;

namespace Tellurium.VisualAssertion.Dashboard.Services.TestCase.Queries
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
            return query.Where(x => x.Project.Id == this.projectId)
                .OrderBy(x=>x.Name)
                .ToList();
        }
    }
}