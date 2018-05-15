using Tellurium.VisualAssertion.Dashboard.Services.WorkSeed;


namespace Tellurium.VisualAssertion.Dashboard.Services.Commands.MarkAsPattern
{
    public class MarkAsPatternCommand:ICommand
    {
        public long TestResultId { get; }

        public MarkAsPatternCommand(long testResultId)
        {
            TestResultId = testResultId;
        }
    }
}