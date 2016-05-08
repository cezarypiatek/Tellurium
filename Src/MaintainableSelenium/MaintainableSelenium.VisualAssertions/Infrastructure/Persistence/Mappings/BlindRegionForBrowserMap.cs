using FluentNHibernate.Mapping;
using MaintainableSelenium.VisualAssertions.Screenshots.Domain;

namespace MaintainableSelenium.VisualAssertions.Infrastructure.Persistence.Mappings
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