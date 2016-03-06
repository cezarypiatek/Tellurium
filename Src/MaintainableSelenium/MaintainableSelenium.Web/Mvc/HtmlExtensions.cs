using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Routing;

namespace MaintainableSelenium.Web.Mvc
{
    public static class HtmlExtensions
    {
        public static string ActionFor<TController>(this  UrlHelper urlHelper, Expression<Action<TController>> action) where TController:Controller
        {
            RouteValueDictionary valuesFromExpression = Microsoft.Web.Mvc.Internal.ExpressionHelper.GetRouteValuesFromExpression(action);
            return urlHelper.RouteUrl(valuesFromExpression);
        }
    }
}