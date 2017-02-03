using System.Linq;
using Tellurium.VisualAssertions.Infrastructure;
using Tellurium.VisualAssertions.Screenshots.Domain;

namespace Tellurium.VisualAssertions.Screenshots.Queries
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