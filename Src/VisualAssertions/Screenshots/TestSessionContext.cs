using System;
using Tellurium.VisualAssertions.Screenshots.Utils;

namespace Tellurium.VisualAssertions.Screenshots
{
    public class TestSessionContext
    {
        private static TestSessionContext current;
        private static object contextLockObject = new object();

        public static TestSessionContext Current
        {
            get
            {
                if (current == null)
                {
                    lock (contextLockObject)
                    {
                        if (current == null)
                        {
                            current = new TestSessionContext()
                            {
                                StartDate = DateTime.Now.TrimToSeconds()
                            };
                        }
                    }
                }
                return current;
            }
            set => current = value;
        }

        public DateTime StartDate { get; private set; }
    }
}