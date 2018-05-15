using Tellurium.VisualAssertions.Infrastructure;

namespace Tellurium.VisualAssertion.Dashboard.Services.WorkSeed.CommandDispathers
{
    public class TransactionalCommandDispather:ICommandDispatcher
    {
        private readonly ICommandDispatcher wrappedDispatcher;
        private readonly ISessionContext sessionContext;

        public TransactionalCommandDispather(ICommandDispatcher wrappedDispatcher,  ISessionContext sessionContext)
        {
            this.wrappedDispatcher = wrappedDispatcher;
            this.sessionContext = sessionContext;
        }

        public void Execute<TCommand>(TCommand command) where TCommand : ICommand
        {
            using (var session = sessionContext.Session)
            {
                using (var tx = session.BeginTransaction())
                {
                    wrappedDispatcher.Execute(command);
                    tx.Commit();
                }    
            }
        }
    }
}