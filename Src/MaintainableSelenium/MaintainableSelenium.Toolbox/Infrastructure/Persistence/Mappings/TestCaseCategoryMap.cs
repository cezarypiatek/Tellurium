using FluentNHibernate.Mapping;
using MaintainableSelenium.Toolbox.Screenshots.Domain;

namespace MaintainableSelenium.Toolbox.Infrastructure.Persistence.Mappings
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