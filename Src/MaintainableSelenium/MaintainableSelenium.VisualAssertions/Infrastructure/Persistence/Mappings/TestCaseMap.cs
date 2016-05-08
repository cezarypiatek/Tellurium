using FluentNHibernate.Mapping;
using MaintainableSelenium.VisualAssertions.Screenshots.Domain;

namespace MaintainableSelenium.VisualAssertions.Infrastructure.Persistence.Mappings
{
    public class TestCaseMap : ClassMap<TestCase>
    {
        public TestCaseMap()
        {
            Id(x => x.Id);
            Map(x => x.PatternScreenshotName);
            References(x => x.Category);
            HasMany(x => x.Patterns).Cascade.Persist();
            References(x => x.Project);
        }
    }
}