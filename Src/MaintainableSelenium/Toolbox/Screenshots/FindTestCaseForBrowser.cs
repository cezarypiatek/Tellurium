using System.Linq;

namespace MaintainableSelenium.Toolbox.Screenshots
{
    public class FindTestCaseForBrowser:IQueryOne<TestCase>
    {
        private string testName;
        private string screenshotName;
        private string browserName;

        public static FindTestCaseForBrowser Create(string testName, string screenshotName, string browserName)
        {
            var query = new FindTestCaseForBrowser
            {
                testName = testName,
                screenshotName = screenshotName,
                browserName = browserName
            };
            return query;
        }

        public TestCase GetQuery(IQueryable<TestCase> query)
        {
            return query
                .First(x => x.TestName == testName && x.PatternScreenshotName == screenshotName &&
                            x.BrowserName == browserName);

        }
    }
}