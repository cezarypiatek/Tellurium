using System;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.Core;
using Castle.DynamicProxy;
using Castle.MicroKernel.Proxy;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using MaintainableSelenium.Toolbox;
using MaintainableSelenium.Toolbox.Infrastructure;
using MaintainableSelenium.Web.Mvc;
using NHibernate;
using NHibernate.Context;
using IInterceptor = Castle.DynamicProxy.IInterceptor;

namespace MaintainableSelenium.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static WindsorContainer container;

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            container = new WindsorContainer();
            container.Register(
               Classes.FromAssemblyContaining<WebAssemblyIdentity>()
                   .BasedOn<Controller>()
                   .LifestyleTransient(),
               Classes.FromAssemblyContaining<WebAssemblyIdentity>()
                    .Where(type => type.GetInterfaces().Any())
                    .WithServiceAllInterfaces()
                    .LifestyleSingleton(),
                Classes.FromAssemblyContaining<ToolboxAssemblyIdentity>()
                    .Where(type => type.GetInterfaces().Any())
                    .WithServiceAllInterfaces()
                    .LifestyleSingleton(),
                Component.For<ISessionFactory>()
                    .UsingFactoryMethod(kernel=> PersistanceEngine.CreateSessionFactory<WebSessionContext>())
                    .LifestyleSingleton()
                    );

            ControllerBuilder.Current.SetControllerFactory(new WindsorControllerFactory(container));
        }

        protected void Application_End()
        {
            container.Dispose();
        }
    }
}
