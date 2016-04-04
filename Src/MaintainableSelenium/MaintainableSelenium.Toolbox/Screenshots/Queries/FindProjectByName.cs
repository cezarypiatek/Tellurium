using System.Linq;
using MaintainableSelenium.Toolbox.Infrastructure;
using MaintainableSelenium.Toolbox.Screenshots.Domain;

namespace MaintainableSelenium.Toolbox.Screenshots.Queries
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