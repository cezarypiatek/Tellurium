using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Sample.Website.Startup))]
namespace Sample.Website
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
