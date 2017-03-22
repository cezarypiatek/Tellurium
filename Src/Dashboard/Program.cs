using System;
using System.Threading;
using Topshelf;

namespace Tellurium.VisualAssertion.Dashboard
{
    public class Program
    {
        public static void Main(string[] args)
        {
#if DEBUG
            using (var server = new WebServer())
            {
                server.Run(consoleMode:true);
                Console.ReadKey();
            }
#else
            InstallDashboardService();
#endif
        }

        private static void InstallDashboardService()
        {
            HostFactory.Run(hostConfiguration =>
            {
                hostConfiguration.Service<WebServer>(wsc =>
                {
                    wsc.ConstructUsing(() => new WebServer());
                    wsc.WhenStarted(server =>
                    {
                       server.Run();
                    });
                    wsc.WhenStopped(ws => ws.Dispose());
                });
                hostConfiguration.RunAsLocalSystem();
                hostConfiguration.SetDescription("This is Tellurium Dashboard");
                hostConfiguration.SetDisplayName("Tellurium Dashboard");
                hostConfiguration.SetServiceName("TelluriumDashboard");
                hostConfiguration.StartAutomatically();
            });
        }
    }
}
