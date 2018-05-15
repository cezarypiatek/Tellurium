using Tellurium.VisualAssertion.Dashboard.Services.TestResults;
using Tellurium.VisualAssertion.Dashboard.Services.WorkSeed;
using Tellurium.VisualAssertions.Infrastructure;
using Tellurium.VisualAssertions.Screenshots.Domain;

namespace Tellurium.VisualAssertion.Dashboard.Services.Queries
{
    public class GetProjectListQuery:IQuery
    {
        
    }

    public class GetProjectListQueryHandler:IQueryHandler<GetProjectListQuery, ProjectListViewModel>
    {
        private readonly IRepository<Project> projectRepository;

        public GetProjectListQueryHandler(IRepository<Project> projectRepository)
        {
            this.projectRepository = projectRepository;
        }

        public ProjectListViewModel Execute(GetProjectListQuery query)
        {
            var projects = projectRepository.GetAll();
            return new ProjectListViewModel()
            {
                Projects = projects.ConvertAll(x=> new ProjectListItemDTO
                {
                    ProjectName = x.Name,
                    ProjectId = x.Id
                })
            };
        }
    }
}