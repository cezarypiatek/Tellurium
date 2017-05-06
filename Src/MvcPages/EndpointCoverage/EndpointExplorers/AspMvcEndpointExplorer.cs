using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Tellurium.MvcPages.WebPages;

namespace Tellurium.MvcPages.EndpointCoverage.EndpointExplorers
{
    public class AspMvcEndpointExplorer:IEndpointExplorer
    {
        private readonly IReadOnlyList<Assembly> availableEndpointsAssemblies;
        

        public AspMvcEndpointExplorer(IReadOnlyList<Assembly> availableEndpointsAssemblies)
        {
            this.availableEndpointsAssemblies = availableEndpointsAssemblies;
        }

        public IEnumerable<string> GetAvailableEndpoints()
        {
            foreach (var endpointsAssembly in availableEndpointsAssemblies)
            {
                foreach (var endpoint in GetAvailablePagesFromAssembly(endpointsAssembly))
                {
                    yield return MvcEndpointsHelper.NormalizeEndpointAddress(endpoint);
                }
            }
        }

        private static IReadOnlyList<MethodInfo> GetActionsMethods(Type t)
        {
            return t.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                .Where(m => m.IsSpecialName == false)
                .ToList().AsReadOnly();
        }

        private static string GetActionUrl(Type controller, MethodInfo action)
        {
            var areaName = UrlHelper.GetAreaName(controller);
            var controllerName = UrlHelper.GetControllerName(controller);
            var actionName = action.Name;
            return String.Join("/", areaName, controllerName, actionName).TrimEnd('/');
        }

        static IReadOnlyList<Type> GetAllControllers(Assembly assembly)
        {
            
            return assembly.GetTypes().Where(InheritFromController).ToList();
        }

        const string AspControllerNamespace = "System.Web.Mvc.Controller";
        private const string AspCoreControllerNamespace = "Microsoft.AspNetCore.Mvc";

        private static bool InheritFromController(Type t)
        {
            if (t == typeof(object) || t.BaseType == null)
            {
                return false;
            }
            
            if (t.BaseType.Namespace == AspControllerNamespace || t.BaseType.Namespace == AspCoreControllerNamespace)
            {
                return true;
            }
            return InheritFromController(t.BaseType);
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
    }
}