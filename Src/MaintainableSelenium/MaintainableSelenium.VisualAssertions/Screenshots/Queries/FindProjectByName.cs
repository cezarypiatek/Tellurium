using System.Linq;
using MaintainableSelenium.VisualAssertions.Infrastructure;
using MaintainableSelenium.VisualAssertions.Screenshots.Domain;

namespace MaintainableSelenium.VisualAssertions.Screenshots.Queries
{
    public class FindProjectByName : IQueryOne<Project>
    {
        private readonly string projectName;

        public FindProjectByName(string projectName)
        {
            this.projectName = projectName;
        }

        public Project GetQuery(IQueryable<Project> query)
        {
            return query.FirstOrDefault(x => x.Name == projectName);
        }
    }
}