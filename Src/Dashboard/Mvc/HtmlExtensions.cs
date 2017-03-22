using System;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using Tellurium.VisualAssertion.Dashboard.Mvc.Utils;

namespace Tellurium.VisualAssertion.Dashboard.Mvc
{
    public static class HtmlExtensions
    {
        public static string ActionFor<TController>(this IUrlHelper urlHelper, Expression<Action<TController>> action) where TController:Controller
        {
            RouteValueDictionary valuesFromExpression = TelluriumExpressionHelper.GetRouteValuesFromExpression(action);
            return urlHelper.RouteUrl(valuesFromExpression);
        }  
       
    
        public static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static HtmlString GetAppVersion()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            return new  HtmlString(version);
        }
    }
}