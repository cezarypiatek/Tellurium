using System;
using System.Diagnostics;
using System.Reflection;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;
using NHibernate;
using NHibernate.Tool.hbm2ddl;

namespace MaintainableSelenium.Toolbox.Screenshots
{
    public class PersistanceEngine
    {
        [ThreadStatic]
        private static ISessionFactory sessionFactory;

        [ThreadStatic] private static ISession session;

        public static ISessionFactory CreateSessionFactory()
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
                .BuildConfiguration()
                .BuildSessionFactory();
        }

        public static ISession GetSession()
        {
            if (sessionFactory == null)
            {
                sessionFactory = CreateSessionFactory();
                session = sessionFactory.OpenSession();
            }
            return session;
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