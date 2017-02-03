using System;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.Windsor;

namespace Tellurium.VisualAssertions.Dashboard.Mvc
{
    public class WindsorControllerFactory : DefaultControllerFactory
    {
        readonly IWindsorContainer container;

        public WindsorControllerFactory(IWindsorContainer container)
        {
            this.container = container;
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            if (controllerType != null && container.Kernel.HasComponent(controllerType))
                return (IController) container.Resolve(controllerType);

            return base.GetControllerInstance(requestContext, controllerType);
        }

        public override void ReleaseController(IController controller)
        {
            container.Release(controller);
        }
    }
}