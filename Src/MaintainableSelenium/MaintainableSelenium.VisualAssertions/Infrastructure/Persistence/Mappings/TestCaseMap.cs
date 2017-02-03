using FluentNHibernate.Mapping;
using Tellurium.VisualAssertions.Screenshots.Domain;

namespace Tellurium.VisualAssertions.Infrastructure.Persistence.Mappings
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