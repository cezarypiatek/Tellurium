using System.Collections.Generic;
using MaintainableSelenium.Web.Models.Home;

namespace MaintainableSelenium.Web.Models.TestCase
{
    public class ExtendedTestCaseInfo
    {
        public string TestCaseName { get; set; }
        public List<TestCaseShortcut> Browsers { get; set; }
    }
}