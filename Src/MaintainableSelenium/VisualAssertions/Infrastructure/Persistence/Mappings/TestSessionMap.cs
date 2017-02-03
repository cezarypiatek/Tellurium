using FluentNHibernate.Mapping;
using Tellurium.VisualAssertions.Screenshots.Domain;

namespace Tellurium.VisualAssertions.Infrastructure.Persistence.Mappings
{
    public class TestSessionMap : ClassMap<TestSession>
    {
        public TestSessionMap()
        {
            Id(x => x.Id);
            Map(x => x.StartDate);
            References(x => x.Project);
            HasMany(x => x.TestResults).Cascade.AllDeleteOrphan();
            HasMany(x => x.Browsers).Element("Value").Cascade.AllDeleteOrphan();
        }
    }
}