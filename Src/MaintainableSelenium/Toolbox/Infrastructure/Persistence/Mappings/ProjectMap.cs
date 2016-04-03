using FluentNHibernate.Mapping;
using MaintainableSelenium.Toolbox.Screenshots.Domain;

namespace MaintainableSelenium.Toolbox.Infrastructure.Persistence.Mappings
{
    public class ProjectMap : ClassMap<Project>
    {
        public ProjectMap()
        {
            Id(x => x.Id);
            Map(x => x.Name);
            HasMany(x=> x.GlobalBlindRegionsForBrowsers).Cascade.AllDeleteOrphan();
            HasMany(x => x.TestCases).Cascade.Persist(); 
            HasMany(x => x.Sessions).Cascade.Persist();
        }
    }
}