using System.Collections.Generic;
using System.Linq;
using MaintainableSelenium.VisualAssertions.Infrastructure;

namespace MaintainableSelenium.VisualAssertions.Screenshots.Domain
{
    public class TestCaseCategory : Entity, IBlindRegionForBrowserOwner
    {
        public virtual string Name { get; set; }
        public virtual Project Project { get; set; }
        public virtual IList<TestCase> TestCases { get; set; }
        public virtual IList<BlindRegionForBrowser> CategoryBlindRegionsForBrowsers { get; set; }


        public TestCaseCategory()
        {
            TestCases = new List<TestCase>();
            CategoryBlindRegionsForBrowsers = new List<BlindRegionForBrowser>();
        }

        public virtual TestCase AddTestCase(string testCaseName)
        {
            var newTestCase = new TestCase
            {
                PatternScreenshotName = testCaseName,
                Category = this,
                Project = this.Project
            };
            TestCases.Add(newTestCase);
            return newTestCase;
        }

        public virtual IReadOnlyList<BlindRegion> GetAllBlindRegionsForBrowser(string browserName)
        {
            var result = new List<BlindRegion>();
            var projectLevelBlindRegions = this.Project.GetBlindRegionsForBrowser(browserName);
            result.AddRange(projectLevelBlindRegions);
            var categoryLevelBlindRegionsForBrowser = this.CategoryBlindRegionsForBrowsers.FirstOrDefault(x => x.BrowserName == browserName);
            if (categoryLevelBlindRegionsForBrowser != null)
            {
                result.AddRange(categoryLevelBlindRegionsForBrowser.BlindRegions);
            }
            return result.AsReadOnly();
        }

        public virtual BlindRegionForBrowser GetOwnBlindRegionForBrowser(string browserName)
        {
            return CategoryBlindRegionsForBrowsers.FirstOrDefault(x => x.BrowserName == browserName);
        }

        public virtual void AddBlindRegionForBrowser(BlindRegionForBrowser blindRegionForBrowser)
        {
            this.CategoryBlindRegionsForBrowsers.Add(blindRegionForBrowser);
        }
    }
}