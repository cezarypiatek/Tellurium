using FluentNHibernate.Mapping;
using MaintainableSelenium.Toolbox.Screenshots.Domain;

namespace MaintainableSelenium.Toolbox.Infrastructure.Persistence.Mappings
{
    public class TestCaseMap : ClassMap<TestCase>
    {
        public TestCaseMap()
        {
            Id(x => x.Id);
            Map(x => x.PatternScreenshotName);
            Map(x => x.Category);
            HasMany(x => x.Patterns).Cascade.Persist();
            References(x => x.Project);
        }
    }
}