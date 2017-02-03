using Microsoft.Owin;
using Owin;
using Tellurium.Sample.Website;

[assembly: OwinStartup(typeof(Startup))]
namespace Tellurium.Sample.Website
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
