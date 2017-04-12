using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Tellurium.VisualAssertion.Dashboard
{
    public class WebServer:IDisposable
    {
        private IWebHost host;

        public void Run(bool consoleMode=false)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .Build();

            host = new WebHostBuilder()
                .UseKestrel()
                .UseConfiguration(config)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .Build();

            if (consoleMode)
            {
                host.Run();
            }
            else
            {
                host.Start();
            }
        }

        public void Dispose()
        {
            host?.Dispose();
        }
    }
}