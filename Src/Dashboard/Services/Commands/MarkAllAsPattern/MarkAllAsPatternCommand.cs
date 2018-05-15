using Tellurium.VisualAssertion.Dashboard.Services.WorkSeed;

namespace Tellurium.VisualAssertion.Dashboard.Services.Commands.MarkAllAsPattern
{
    public class MarkAllAsPatternCommand:ICommand
    {
        public long TestSessionId { get; }
        public string BrowserName { get; }

        public MarkAllAsPatternCommand(long testSessionId, string browserName)
        {
            this.TestSessionId = testSessionId;
            this.BrowserName = browserName;
        }
    }
}