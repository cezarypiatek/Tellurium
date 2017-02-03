using System;

namespace Tellurium.VisualAssertions.Screenshots
{
    public class TestSessionContext
    {
        private static TestSessionContext current;

        public static TestSessionContext Current
        {
            get
            {
                if (current == null)
                {
                    current = new TestSessionContext()
                    {
                        StartDate = DateTime.Now
                    };
                }
                return current;
            }
            set { current = value; }
        }

        public DateTime StartDate { get; set; }
    }
}