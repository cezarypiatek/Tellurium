using FluentNHibernate.Mapping;
using MaintainableSelenium.Toolbox.Screenshots.Domain;

namespace MaintainableSelenium.Toolbox.Infrastructure.Persistence.Mappings
{
    public class TestResultMap : ClassMap<TestResult>
    {
        public TestResultMap()
        {
            Id(x => x.Id);
            Map(x => x.ScreenshotName);
            Map(x => x.BrowserName);
            Map(x => x.TestPassed);
            References(x => x.ErrorScreenshot).Cascade.All();
            References(x => x.Pattern);
            References(x => x.TestSession);
        }
    }
}