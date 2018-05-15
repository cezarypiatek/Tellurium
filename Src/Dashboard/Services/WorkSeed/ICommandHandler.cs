using Castle.MicroKernel;
using Tellurium.VisualAssertions.Infrastructure;

namespace Tellurium.VisualAssertion.Dashboard.Services.WorkSeed
{
    public interface ICommandHandler<in TCommand> where TCommand : ICommand
    {
        void Excute(TCommand command);
    }

    public interface IQuery{}

    public interface IQueryHandler<in TQuery, out TResult> where TQuery:IQuery
    {
        TResult Execute(TQuery query);
    }

    public interface IQueryDispatcher
    {
        TResult Execute<TQuery, TResult>(TQuery query) where TQuery : IQuery;
    }

    public class QueryDispatcher : IQueryDispatcher
    {
        private readonly IKernel kernel;

        public QueryDispatcher(IKernel kernel)
        {
            this.kernel = kernel;
        }


        public TResult Execute<TQuery, TResult>(TQuery query) where TQuery : IQuery
        {
            var handler = kernel.Resolve<IQueryHandler<TQuery, TResult>>();
            return handler.Execute(query);
        }
    }

    public class TransactionalQueryDispatcher:IQueryDispatcher
    {
        private readonly QueryDispatcher dispatcher;
        private readonly ISessionContext sessionContext;

        public TransactionalQueryDispatcher(QueryDispatcher dispatcher, ISessionContext sessionContext )
        {
            this.dispatcher = dispatcher;
            this.sessionContext = sessionContext;
        }

        public TResult Execute<TQuery, TResult>(TQuery query) where TQuery : IQuery
        {
            using (var session = sessionContext.Session)
            {
                using (session.BeginTransaction())
                {
                    return dispatcher.Execute<TQuery,TResult>(query);
                }    
            }
        }
    }
}