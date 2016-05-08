using System.Collections.Generic;
using MaintainableSelenium.VisualAssertions.Web.Models.Home;

namespace MaintainableSelenium.VisualAssertions.Web.Models.TestCase
{
    public class TestCaseListItem
    {
        public string TestCaseName { get; set; }
        public List<BrowserPatternShortcut> Browsers { get; set; }
        public long TestCaseId { get; set; }
    }
}