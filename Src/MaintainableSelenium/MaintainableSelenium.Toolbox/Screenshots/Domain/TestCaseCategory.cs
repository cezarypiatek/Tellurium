using System.Collections.Generic;
using MaintainableSelenium.Toolbox.Infrastructure;

namespace MaintainableSelenium.Toolbox.Screenshots.Domain
{
    public class TestCaseCategory : Entity
    {
        public virtual string Name { get; set; }
        public virtual Project Project { get; set; }
        public virtual IList<TestCase> TestCases { get; set; }

        public TestCaseCategory()
        {
            TestCases = new List<TestCase>();
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
    }
}