using Microsoft.AspNetCore.Http;
using NHibernate.Context;
using NHibernate.Engine;
using ISession = NHibernate.ISession;

namespace Tellurium.VisualAssertion.Dashboard.Mvc.Utils
{
    public class AspCoreSessionContext: CurrentSessionContext
    {
        private static string NhibernateSessionKey = "NhibernateSessionKey";
        private HttpContext HttpContext => ServiceLocator.Resolve<IHttpContextAccessor>().HttpContext;
        
        public AspCoreSessionContext(ISessionFactoryImplementor factory)
        {
        }

        protected override ISession Session
        {
            get
            {
                if (HttpContext.Items.ContainsKey(NhibernateSessionKey) == false)
                {
                    return null;
                }
                return HttpContext.Items[NhibernateSessionKey] as ISession;
                
            }
            set
            {
                HttpContext.Items[NhibernateSessionKey] = value;
                
            }
        }
    }
}