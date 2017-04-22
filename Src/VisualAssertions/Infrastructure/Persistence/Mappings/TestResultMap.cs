using FluentNHibernate.Mapping;
using Tellurium.VisualAssertions.Screenshots.Domain;

namespace Tellurium.VisualAssertions.Infrastructure.Persistence.Mappings
{
    public class TestResultMap : ClassMap<TestResult>
    {
        public TestResultMap()
        {
            Id(x => x.Id);
            Map(x => x.ScreenshotName);
            Map(x => x.BrowserName);
            Map(x => x.Status).CustomType<TestResultStatus>();
            Map(x => x.Category);
            Map(x => x.ErrorScreenshot).Length(int.MaxValue).LazyLoad();
            References(x => x.Pattern);
            References(x => x.TestSession);
            HasManyToMany(x => x.BlindRegionsSnapshot).Cascade.Persist().Table("TestResultBlindRegionsSnapshot");
        }
    }
}