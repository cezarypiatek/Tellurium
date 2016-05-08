using FluentNHibernate.Mapping;
using MaintainableSelenium.VisualAssertions.Screenshots.Domain;

namespace MaintainableSelenium.VisualAssertions.Infrastructure.Persistence.Mappings
{
    public class TestResultMap : ClassMap<TestResult>
    {
        public TestResultMap()
        {
            Id(x => x.Id);
            Map(x => x.ScreenshotName);
            Map(x => x.BrowserName);
            Map(x => x.TestPassed);
            Map(x => x.Category);
            Map(x => x.ErrorScreenshot).Length(int.MaxValue).LazyLoad();
            References(x => x.Pattern);
            References(x => x.TestSession);
        }
    }
}