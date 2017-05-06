using System;
using System.Linq;
using System.Linq.Expressions;

namespace Tellurium.MvcPages.WebPages
{
    internal static class UrlHelper
    {
        public static string BuildActionAddressFromExpression<TController>(Expression<Action<TController>> action)
        {
            var controllerName = action.GetControllerName();
            var actionName = action.GetActionName();
            var areaName = GetAreaName<TController>();
            var actionLink = $"{areaName}/{controllerName}/{actionName}".Trim('/');
            return actionLink;
        }

        private static string GetAreaName<TController>()
        {
            var controllerType = typeof(TController);
            return GetAreaName(controllerType);
        }

        public static string GetAreaName(Type controllerType)
        {
            var areaNameAttribute = controllerType.
                GetCustomAttributesData().
                FirstOrDefault(x => x.AttributeType.Name == "ActionLinkAreaAttribute");

            if (areaNameAttribute == null)
                return string.Empty;
            return areaNameAttribute.ConstructorArguments.FirstOrDefault().Value?.ToString() ?? string.Empty;
        }

        public static string GetControllerName<TController>(this Expression<Action<TController>> action)
        {
            var controllerType = typeof(TController);
            return GetControllerName(controllerType);
        }

        public static string GetControllerName(Type controllerType)
        {
            return controllerType.Name.Replace("Controller", "");
        }

        public static string GetActionName<TController>(this Expression<Action<TController>> action)
        {
            var methodCall = action.Body as MethodCallExpression;
            if (methodCall == null)
            {
                throw new ArgumentException("Invalid method expression");
            }
            return methodCall.Method.Name;
        }
    }
}