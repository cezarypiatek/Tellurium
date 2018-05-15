using System.Collections.Generic;
using Tellurium.VisualAssertion.Dashboard.Models.Home;

namespace Tellurium.VisualAssertion.Dashboard.Models.TestCases
{
    public class TestCaseListItem
    {
        public string TestCaseName { get; set; }
        public List<BrowserPatternShortcut> Browsers { get; set; }
        public long TestCaseId { get; set; }
    }
}