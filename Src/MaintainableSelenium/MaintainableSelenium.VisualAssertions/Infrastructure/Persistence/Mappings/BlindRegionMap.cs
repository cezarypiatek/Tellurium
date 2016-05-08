using FluentNHibernate.Mapping;
using MaintainableSelenium.VisualAssertions.Screenshots.Domain;

namespace MaintainableSelenium.VisualAssertions.Infrastructure.Persistence.Mappings
{
    public class BlindRegionMap : ClassMap<BlindRegion>
    {
        public BlindRegionMap()
        {
            Id(x => x.Id);
            Map(x => x.Left);
            Map(x => x.Top);
            Map(x => x.Width);
            Map(x => x.Height);
        }
    }
}