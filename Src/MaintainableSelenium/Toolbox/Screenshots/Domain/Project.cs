using System.Collections.Generic;

namespace MaintainableSelenium.Toolbox.Screenshots
{
    public class Project
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<TestCaseSet> TestCaseSets { get; set; }
        public List<TestSession> Sessions { get; set; }
    }
}