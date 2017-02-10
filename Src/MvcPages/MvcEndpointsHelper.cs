using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Web.Mvc;

namespace Tellurium.MvcPages
{
    internal static class MvcEndpointsHelper
    {
        private static IReadOnlyList<MethodInfo> GetActionsMethods(Type t)
        {
            return t.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                .Where(m => m.IsSpecialName == false)
                .ToList().AsReadOnly();
        }

        private static string GetAreaName(Type controller)
        {
            var areaAttribute =  controller.GetCustomAttribute(typeof(ActionLinkAreaAttribute)) as ActionLinkAreaAttribute;
            if (areaAttribute == null)
            {
                return string.Empty;
            }
            return areaAttribute.Area;
        }

        private static string GetControllerName(Type controller)
        {
            return controller.Name.Replace("Controller", "");
        }

        private static string GetActionUrl(Type controller, MethodInfo action)
        {
            var areaName = GetAreaName(controller);
            var controllerName = GetControllerName(controller);
            var actionName = action.Name;
            return String.Join("/", areaName, controllerName, actionName).TrimEnd('/');
        }

        static IReadOnlyList<Type> GetAllControllers(Assembly assembly)
        {
            var controllerType = typeof(System.Web.Mvc.Controller);
            return assembly.GetTypes().Where(t=>controllerType.IsAssignableFrom(t)).ToList();
        }

        private static IReadOnlyList<string> GetAllActionsUrlForController(Type controller)
        {
            return GetActionsMethods(controller).Select(m => GetActionUrl(controller, m)).ToList();
        }

        public static IReadOnlyList<string> GetAvailablePagesFromAssembly(Assembly assembly)
        {
            var controllers = GetAllControllers(assembly);
            return controllers.SelectMany(GetAllActionsUrlForController).ToList();
        }

        public static string NormalizeEndpointAddress(string endpoint)
        {
            var address = endpoint.Replace("/Index", "").Trim('/');
            return IdPattern.Replace(address,"");
        }

        private static readonly Regex IdPattern = new Regex(@"/\d+$");
    }
}