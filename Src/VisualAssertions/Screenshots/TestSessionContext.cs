using System;
using Tellurium.VisualAssertions.Screenshots.Utils;

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
                        StartDate = DateTime.Now.TrimToMiliseconds()
                    };
                }
                return current;
            }
            set => current = value;
        }

        public DateTime StartDate { get; private set; }
    }
}