using FluentNHibernate.Mapping;
using MaintainableSelenium.Toolbox.Screenshots;

namespace MaintainableSelenium.Toolbox.Infrastructure.Persistence.Mappings
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