using FluentNHibernate.Mapping;
using Tellurium.VisualAssertions.Screenshots.Domain;

namespace Tellurium.VisualAssertions.Infrastructure.Persistence.Mappings
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