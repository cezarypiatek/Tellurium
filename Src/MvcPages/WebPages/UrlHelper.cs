using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Mvc;
using Microsoft.Web.Mvc;

namespace Tellurium.MvcPages.WebPages
{
    public static class UrlHelper
    {
        public static string BuildActionAddressFromExpression<TController>(Expression<Action<TController>> action) where TController : Controller
        {
            var controllerName = action.GetControllerName();
            var actionName = action.GetActionName();
            var areaName = GetAreaName<TController>();
            var actionLink = string.Format("{0}/{1}/{2}", areaName, controllerName, actionName).Trim('/');
            return actionLink;
        }

        private static string GetAreaName<TController>()
        {
            var controllerType = typeof(TController);
            var areaNameAttribute = controllerType.GetCustomAttribute<ActionLinkAreaAttribute>();
            if (areaNameAttribute == null)
            {
                return string.Empty;
            }
            return areaNameAttribute.Area;
        }

        public static string GetControllerName<TController>(this Expression<Action<TController>> action) where TController : Controller
        {
            var controllerType = typeof(TController);
            return controllerType.Name.Replace("Controller", "");
        }

        public static string GetActionName<TController>(this Expression<Action<TController>> action) where TController : Controller
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