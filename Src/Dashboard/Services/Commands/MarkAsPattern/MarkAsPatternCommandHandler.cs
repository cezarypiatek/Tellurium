using Tellurium.VisualAssertion.Dashboard.Services.WorkSeed;
using Tellurium.VisualAssertions.Infrastructure;
using Tellurium.VisualAssertions.Screenshots.Domain;

namespace Tellurium.VisualAssertion.Dashboard.Services.Commands.MarkAsPattern
{
    public class MarkAsPatternCommandHandler:ICommandHandler<MarkAsPatternCommand>
    {
        private readonly IRepository<TestResult> testRepository;

        public MarkAsPatternCommandHandler(IRepository<TestResult> testRepository)
        {
            this.testRepository = testRepository;
        }

        public void Excute(MarkAsPatternCommand command)
        {
            var testResult = this.testRepository.Get(command.TestResultId);
            testResult.MarkAsPattern();
        }
    }
}