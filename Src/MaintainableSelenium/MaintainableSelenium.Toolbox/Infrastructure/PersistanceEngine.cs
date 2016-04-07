using System.Reflection;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;
using NHibernate;
using NHibernate.Context;
using NHibernate.Tool.hbm2ddl;

namespace MaintainableSelenium.Toolbox.Infrastructure
{
    public class PersistanceEngine
    {
        private static ISessionFactory sessionFactory;
        private static ISessionContext sessionContext;
        
        public static ISessionFactory CreateSessionFactory<TSessionContext>() where TSessionContext : CurrentSessionContext
        {
            return Fluently.Configure()
                //.Database(SQLiteConfiguration.Standard.UsingFile("MaintainableSelenium.db"))
                .Database(MsSqlConfiguration.MsSql2012.ConnectionString("Data Source=localhost\\SQLEXPRESS;Database=MaintainableSelenium;Integrated Security=true;").ShowSql())
                .Mappings(x =>
                {
                    x.FluentMappings.AddFromAssembly(Assembly.GetExecutingAssembly())
                        .Conventions.Add<HiLowIdConvention>();
                    
                })
                .ExposeConfiguration(c => {
                    SchemaMetadataUpdater.QuoteTableAndColumns(c);
                   // new SchemaExport(c).Execute((s) => Debug.WriteLine(s), true, false);
                })
                .CurrentSessionContext<TSessionContext>()
                .BuildConfiguration()
                .BuildSessionFactory();
        }

        public static void InitForUtApplication()
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

    public class HiLowIdConvention:IIdConvention
    {
        public void Apply(IIdentityInstance instance)
        {
            instance.Column("Id");
            instance.GeneratedBy.HiLo("1000");
        }
    }
}