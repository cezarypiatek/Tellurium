using System;
using System.Linq;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.MsDependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NHibernate;
using Tellurium.MvcPages;
using Tellurium.VisualAssertion.Dashboard.Mvc.Utils;
using Tellurium.VisualAssertions;
using Tellurium.VisualAssertions.Infrastructure.Persistence;
using ILoggerFactory = Microsoft.Extensions.Logging.ILoggerFactory;

namespace Tellurium.VisualAssertion.Dashboard
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                // Set a short timeout for easy testing.
                options.IdleTimeout = TimeSpan.FromSeconds(10);
                //options.CookieHttpOnly = true;
            });

            var container = new WindsorContainer();
            container.Register(
               Classes.FromAssemblyContaining<WebAssemblyIdentity>()
                    .Where(type => type.GetInterfaces().Any())
                    .WithServiceAllInterfaces()
                    .LifestyleSingleton(),
                Classes.FromAssemblyContaining<MvcPagesAssemblyIdentity>()
                    .Where(type => type.GetInterfaces().Any())
                    .WithServiceAllInterfaces()
                    .LifestyleSingleton(),
                Classes.FromAssemblyContaining<VisualAssertionsAssemblyIdentity>()
                    .Where(type => type.GetInterfaces().Any())
                    .WithServiceAllInterfaces()
                    .LifestyleSingleton(),
                Component.For<ISessionFactory>()
                    .UsingFactoryMethod(kernel => PersistanceEngine.CreateSessionFactory<AspCoreSessionContext>())
                    .LifestyleSingleton()
                    );
            ServiceLocator.Init(container);
            return WindsorRegistrationHelper.CreateServiceProvider(container, services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddFile("Logs/myapp-{Date}.txt");
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseSession();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
