using Castle.MicroKernel;

namespace Tellurium.VisualAssertion.Dashboard.Services.WorkSeed.CommandDispathers
{
    public class CommandDispather:ICommandDispatcher
    {
        private readonly IKernel kernel;

        public CommandDispather(IKernel kernel)
        {
            this.kernel = kernel;
        }

        public void Execute<TCommand>(TCommand command) where TCommand : ICommand
        {
            var commandHandler = this.kernel.Resolve<ICommandHandler<TCommand>>();
            commandHandler.Excute(command);
        }
    }
}