using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Tellurium.VisualAssertion.Dashboard.Mvc
{
    public static class HtmlExtensions
    {
        public static string ActionFor<TController>(this IUrlHelper urlHelper, Expression<Action<TController>> action) where TController:Controller
        {
            var methodCall = (MethodCallExpression)action.Body;
            var actionName = methodCall.Method.Name;
            var routeValues = BuildParameterValuesFromExpression(methodCall);
            return urlHelper.Action(actionName, GetControllerName<TController>(), routeValues);
        }

        public static HtmlString GetAppVersion()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            return new  HtmlString(version);
        }

        [Pure]
        private static string GetControllerName<TController>() where TController : class
        {
            return typeof(TController).Name.Replace("Controller", "");
        }

        private static RouteValueDictionary BuildParameterValuesFromExpression( MethodCallExpression call)
        {
            var parameterValues = new Dictionary<string, object>();
            var parameters = call.Method.GetParameters();
            if (parameters.Length > 0)
            {
                for (int index = 0; index < parameters.Length; index++)
                {
                    var parameterExpression = call.Arguments[index];
                    var value = Expression.Lambda(parameterExpression)
                        .Compile()
                        .DynamicInvoke();
                    parameterValues.Add(parameters[index].Name, value);
                }
            }
            return new RouteValueDictionary(parameterValues);
        }
    }
}