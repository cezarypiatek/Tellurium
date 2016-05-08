using FluentNHibernate.Mapping;
using MaintainableSelenium.VisualAssertions.Screenshots.Domain;

namespace MaintainableSelenium.VisualAssertions.Infrastructure.Persistence.Mappings
{
    public class ProjectMap : ClassMap<Project>
    {
        public ProjectMap()
        {
            Id(x => x.Id);
            Map(x => x.Name);
            HasManyToMany(x=> x.GlobalBlindRegionsForBrowsers).Cascade.All().Table("ProjectBlindRegionsForBrowser");
            HasMany(x => x.TestCaseCategories).Cascade.Persist(); 
            HasMany(x => x.Sessions).Cascade.Persist();
        }
    }
}