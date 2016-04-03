using FluentNHibernate.Mapping;
using MaintainableSelenium.Toolbox.Screenshots.Domain;

namespace MaintainableSelenium.Toolbox.Infrastructure.Persistence.Mappings
{
    public class BlindRegionForBrowserMap : ClassMap<BlindRegionForBrowser>
    {
        public BlindRegionForBrowserMap()
        {
            Id(x => x.Id);
            Map(x => x.BrowserName);
            HasManyToMany(x => x.BlindRegions).Cascade.AllDeleteOrphan().Table("GlobalBlindRegions");
        }
    }
}