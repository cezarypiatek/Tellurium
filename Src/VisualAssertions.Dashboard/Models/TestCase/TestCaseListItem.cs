using System.Collections.Generic;
using Tellurium.VisualAssertions.Dashboard.Models.Home;

namespace Tellurium.VisualAssertions.Dashboard.Models.TestCase
{
    public class TestCaseListItem
    {
        public string TestCaseName { get; set; }
        public List<BrowserPatternShortcut> Browsers { get; set; }
        public long TestCaseId { get; set; }
    }
}