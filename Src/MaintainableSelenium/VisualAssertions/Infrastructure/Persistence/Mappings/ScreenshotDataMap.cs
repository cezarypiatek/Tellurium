using FluentNHibernate.Mapping;
using Tellurium.VisualAssertions.Screenshots.Domain;

namespace Tellurium.VisualAssertions.Infrastructure.Persistence.Mappings
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