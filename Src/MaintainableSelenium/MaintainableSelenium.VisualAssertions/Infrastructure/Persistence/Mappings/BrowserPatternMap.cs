using FluentNHibernate.Mapping;
using MaintainableSelenium.VisualAssertions.Screenshots.Domain;

namespace MaintainableSelenium.VisualAssertions.Infrastructure.Persistence.Mappings
{
    public class BrowserPatternMap : ClassMap<BrowserPattern>
    {
        public BrowserPatternMap()
        {
            Id(x => x.Id);
            Map(x => x.BrowserName);
            Map(x => x.IsActive);
            HasManyToMany(x => x.BlindRegions).Cascade.AllDeleteOrphan().Table("LocalBlindRegions");
            References(x => x.PatternScreenshot).Cascade.All();
            References(x => x.TestCase);
        }
    }
}