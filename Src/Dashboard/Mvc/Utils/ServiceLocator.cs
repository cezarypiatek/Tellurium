using Castle.Windsor;

namespace Tellurium.VisualAssertion.Dashboard.Mvc.Utils
{
    public class ServiceLocator
    {
        private static IWindsorContainer _container;

        public static void Init(IWindsorContainer container)
        {
            _container = container;
        }

        public static T Resolve<T>()
        {
            return _container.Resolve<T>();
        }
    }
}