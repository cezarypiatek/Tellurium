namespace Tellurium.VisualAssertion.Dashboard.Services.WorkSeed
{
    public interface ICommandDispatcher
    {
        void Execute<TCommand>(TCommand command) where TCommand:ICommand;
    }
}