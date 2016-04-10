using System.Reflection;
using FluentNHibernate.Cfg;
using MaintainableSelenium.Toolbox.Infrastructure.Persistence.Conventions;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Context;
using NHibernate.Tool.hbm2ddl;

namespace MaintainableSelenium.Toolbox.Infrastructure.Persistence
{
    public static class PersistanceEngine
    {
        private static ISessionFactory sessionFactory;
        private static ISessionContext sessionContext;
        
        public static ISessionFactory CreateSessionFactory<TSessionContext>() where TSessionContext : CurrentSessionContext
        {
            var nhDbConfig = ReadNhConfig();
            return Fluently.Configure(nhDbConfig)
                .Mappings(x =>
                {
                    x.FluentMappings.AddFromAssembly(Assembly.GetExecutingAssembly())
                        .Conventions.Add<HiLowIdConvention>();
                    
                })
                .ExposeConfiguration(c => {
                    SchemaMetadataUpdater.QuoteTableAndColumns(c);
                    new SchemaUpdate(c).Execute(false, true);
                })
                .CurrentSessionContext<TSessionContext>()
                .BuildConfiguration()
                .BuildSessionFactory();
        }

        private static Configuration ReadNhConfig()
        {
            var nhDbConfig = new Configuration();
            nhDbConfig.Configure();
            return nhDbConfig;
        }

        private static void InitForUtApplication()
        {
            sessionFactory = CreateSessionFactory<ThreadStaticSessionContext>();
            sessionContext = new SessionContext(sessionFactory);
        }

        public static ISession GetSession()
        {
            if (sessionFactory == null)
            {
                InitForUtApplication();
            }
            return sessionContext.Session;
        }

        internal static ISessionContext GetSessionContext()
        {
            if (sessionContext == null)
            {
                InitForUtApplication();
            }
            return sessionContext;
        }
    }
}