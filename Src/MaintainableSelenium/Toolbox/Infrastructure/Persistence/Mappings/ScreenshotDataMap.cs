using FluentNHibernate.Mapping;
using MaintainableSelenium.Toolbox.Screenshots.Domain;

namespace MaintainableSelenium.Toolbox.Infrastructure.Persistence.Mappings
{
    public class ScreenshotDataMap : ClassMap<ScreenshotData>
    {
        public ScreenshotDataMap()
        {
            Id(x => x.Id);
            Map(x => x.Hash);
            Map(x => x.Image).Length(int.MaxValue);
        }
    }
}