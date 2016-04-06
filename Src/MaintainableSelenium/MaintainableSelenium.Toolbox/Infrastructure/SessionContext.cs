using NHibernate;
using NHibernate.Context;

namespace MaintainableSelenium.Toolbox.Infrastructure
{
    public class SessionContext : ISessionContext
    {
        private readonly ISessionFactory sessionFactory;

        public SessionContext(ISessionFactory sessionFactory)
        {
            this.sessionFactory = sessionFactory;
        }

        public ISession Session
        {
            get
            {
                if (CurrentSessionContext.HasBind(sessionFactory) == false)
                {
                    var session = sessionFactory.OpenSession();
                    session.FlushMode = FlushMode.Commit;
                    CurrentSessionContext.Bind(session);
                }
                return sessionFactory.GetCurrentSession();
            }
        }
    }

    public interface ISessionContext
    {
        ISession Session { get; }
    }
}