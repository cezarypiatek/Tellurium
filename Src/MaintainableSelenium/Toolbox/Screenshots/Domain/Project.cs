using System.Collections.Generic;

namespace MaintainableSelenium.Toolbox.Screenshots
{
    public class Project:Entity
    {
        public string Name { get; set; }
        public TestCaseSet TestCaseSet { get; set; }
        public List<TestSession> Sessions { get; set; }

        public void AddSession(TestSession session)
        {
            session.Project = this;
            Sessions.Add(session);
        }

        public Project()
        {
           Sessions = new List<TestSession>(); 
           TestCaseSet = new TestCaseSet();
        }
    }
}