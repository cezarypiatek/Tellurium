using System;
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

        static ISessionFactory CreateSessionFactory<TSessionContext>() where TSessionContext : CurrentSessionContext
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

        public static void InitForWebApplication()
        {
            sessionFactory = CreateSessionFactory<WebSessionContext>();
        }

        public static void InitForUtApplication()
        {
            sessionFactory = CreateSessionFactory<ThreadStaticSessionContext>();
        }

        public static ISession GetSession()
        {
            if (sessionFactory == null)
            {
                InitForUtApplication();
            }

            if (CurrentSessionContext.HasBind(sessionFactory))
            {
                return sessionFactory.GetCurrentSession();
            }
            var session = sessionFactory.OpenSession();
            CurrentSessionContext.Bind(session);
            return session;
        }

        public static void Unbind()
        {
            CurrentSessionContext.Unbind(sessionFactory);
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