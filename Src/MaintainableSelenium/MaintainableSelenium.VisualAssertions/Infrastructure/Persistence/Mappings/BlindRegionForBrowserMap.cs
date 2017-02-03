using FluentNHibernate.Mapping;
using Tellurium.VisualAssertions.Screenshots.Domain;

namespace Tellurium.VisualAssertions.Infrastructure.Persistence.Mappings
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