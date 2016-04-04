using System.Web.Mvc;
using System.Web.Routing;
using MaintainableSelenium.Toolbox.Infrastructure;

namespace MaintainableSelenium.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            PersistanceEngine.InitForWebApplication();
        }
    }
}
