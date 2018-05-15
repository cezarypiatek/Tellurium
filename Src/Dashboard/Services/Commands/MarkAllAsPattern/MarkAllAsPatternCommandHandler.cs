using Tellurium.VisualAssertion.Dashboard.Services.TestResults.Queries;
using Tellurium.VisualAssertion.Dashboard.Services.WorkSeed;
using Tellurium.VisualAssertions.Infrastructure;
using Tellurium.VisualAssertions.Screenshots.Domain;

namespace Tellurium.VisualAssertion.Dashboard.Services.Commands.MarkAllAsPattern
{
    public class MarkAllAsPatternCommandHandler:ICommandHandler<MarkAllAsPatternCommand>
    {
        private readonly IRepository<TestResult> testRepository;

        public MarkAllAsPatternCommandHandler(IRepository<TestResult> testRepository)
        {
            this.testRepository = testRepository;
        }

        public void Excute(MarkAllAsPatternCommand command)
        {
            var failedTestResultsQuery = new FindFailedTestFromSessionForBrowser(command.TestSessionId, command.BrowserName);
            var failedTestResults = this.testRepository.FindAll(failedTestResultsQuery);
            foreach (var testResult in failedTestResults)
            {
                testResult.MarkAsPattern();
            }
        }
    }
}