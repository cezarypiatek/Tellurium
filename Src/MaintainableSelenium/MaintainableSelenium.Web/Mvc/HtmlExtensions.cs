using System;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
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
        
        public static IHtmlString ActionLink<TController>(this  AjaxHelper ajaxHelper, Expression<Action<TController>> action, string text, string updateTargetId, object htmlAttributes=null) where TController:Controller
        {
            RouteValueDictionary valuesFromExpression = Microsoft.Web.Mvc.Internal.ExpressionHelper.GetRouteValuesFromExpression(action);

            return ajaxHelper.ActionLink(text, action.Name, valuesFromExpression, new AjaxOptions() { HttpMethod = "GET", UpdateTargetId = updateTargetId }, new RouteValueDictionary(htmlAttributes));
        }
        
        public static IHtmlString ChangeActionLink<TController>(this  AjaxHelper ajaxHelper, Expression<Action<TController>> action, string text, string confirmMessage, string refreshElementId =null, object htmlAttributes=null) where TController:Controller
        {
            RouteValueDictionary valuesFromExpression = Microsoft.Web.Mvc.Internal.ExpressionHelper.GetRouteValuesFromExpression(action);

            var ajaxOptions = new AjaxOptions() { HttpMethod = "POST", Confirm = confirmMessage,};
            if (string.IsNullOrWhiteSpace(refreshElementId) == false)
            {
                ajaxOptions.UpdateTargetId = refreshElementId;
                ajaxOptions.InsertionMode = InsertionMode.ReplaceWith;
            }
            return ajaxHelper.ActionLink(text, action.Name, valuesFromExpression, ajaxOptions, new RouteValueDictionary(htmlAttributes));
        }

        public static string GetCurrentActionName()
        {
            return HttpContext.Current.Request.RequestContext.RouteData.Values["action"] as string;
        }
    }
}