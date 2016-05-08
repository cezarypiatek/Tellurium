using FluentNHibernate.Mapping;
using MaintainableSelenium.VisualAssertions.Screenshots.Domain;

namespace MaintainableSelenium.VisualAssertions.Infrastructure.Persistence.Mappings
{
    public class TestCaseCategoryMap : ClassMap<TestCaseCategory>
    {
        public TestCaseCategoryMap()
        {
            Id(x => x.Id);
            Map(x => x.Name);
            References(x => x.Project);
            HasManyToMany(x => x.CategoryBlindRegionsForBrowsers).Cascade.All().Table("CategoryBlindRegionsForBrowser");
            HasMany(x => x.TestCases).Cascade.AllDeleteOrphan();
        }
    }
}