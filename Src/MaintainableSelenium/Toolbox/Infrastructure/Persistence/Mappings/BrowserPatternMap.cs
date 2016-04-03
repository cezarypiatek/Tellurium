using FluentNHibernate.Mapping;
using MaintainableSelenium.Toolbox.Screenshots.Domain;

namespace MaintainableSelenium.Toolbox.Infrastructure.Persistence.Mappings
{
    public class BrowserPatternMap : ClassMap<BrowserPattern>
    {
        public BrowserPatternMap()
        {
            Id(x => x.Id);
            Map(x => x.BrowserName);
            HasManyToMany(x => x.BlindRegions).Cascade.AllDeleteOrphan().Table("LocalBlindRegions");
            References(x => x.PatternScreenshot).Cascade.All();
            References(x => x.TestCase);
        }
    }
}