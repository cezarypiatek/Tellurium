using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Microsoft.AspNetCore.Routing;

namespace Tellurium.VisualAssertion.Dashboard.Mvc.Utils
{
    public static class TelluriumExpressionHelper
    {
        public static RouteValueDictionary GetRouteValuesFromExpression<TController>(Expression<Action<TController>> action) where TController : Controller
        {
            if (action == null)
                throw new ArgumentNullException("action");
            MethodCallExpression body = action.Body as MethodCallExpression;
            if (body == null)
                throw new ArgumentException("Expression must be a method call", "action");
            string name = typeof(TController).Name;
            if (!name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Target object must end with 'Controller'", "action");
            string str = name.Substring(0, name.Length - "Controller".Length);
            if (str.Length == 0)
                throw new ArgumentException("Invalid controller name", "action");
            string targetActionName = GetTargetActionName(body.Method);
            RouteValueDictionary rvd = new RouteValueDictionary();
            rvd.Add("Controller", (object)str);
            rvd.Add("Action", (object)targetActionName);
            //var linkAreaAttribute = ((IEnumerable<object>)typeof(TController).GetCustomAttributes(typeof(ActionLinkAreaAttribute), true)).FirstOrDefault<object>() as ActionLinkAreaAttribute;
            //if (linkAreaAttribute != null)
            //{
            //    string area = linkAreaAttribute.Area;
            //    rvd.Add("Area", (object)area);
            //}
            AddParameterValuesFromExpressionToDictionary(rvd, body);
            return rvd;
        }

        private static string GetTargetActionName(MethodInfo methodInfo)
        {
            string name = methodInfo.Name;
            if (methodInfo.IsDefined(typeof(NonActionAttribute), true))
                throw new InvalidOperationException("Given method is not an action");
            ActionNameAttribute actionNameAttribute = methodInfo.GetCustomAttributes(typeof(ActionNameAttribute), true).OfType<ActionNameAttribute>().FirstOrDefault<ActionNameAttribute>();
            if (actionNameAttribute != null)
                return actionNameAttribute.Name;
            return name;
        }

        private static void AddParameterValuesFromExpressionToDictionary(RouteValueDictionary rvd, MethodCallExpression call)
        {
            ParameterInfo[] parameters = call.Method.GetParameters();
            if (parameters.Length <= 0)
                return;
            for (int index = 0; index < parameters.Length; ++index)
            {
                Expression expression = call.Arguments[index];
                ConstantExpression constantExpression = expression as ConstantExpression;
                object obj = constantExpression == null ? Evaluate(expression) : constantExpression.Value;
                rvd.Add(parameters[index].Name, obj);
            }
        }

        public static object Evaluate(Expression arg)
        {
            if (arg == null)
            {
                throw new ArgumentNullException("arg");
            }

            Func<object, object> func = Wrap(arg);
            return func(null);
        }

        private static Func<object, object> Wrap(Expression arg)
        {
            Expression<Func<object, object>> lambdaExpr = Expression.Lambda<Func<object, object>>(Expression.Convert(arg, typeof(object)), _unusedParameterExpr);
            return CachedExpressionCompiler.Process(lambdaExpr);
        }

        private static readonly ParameterExpression _unusedParameterExpr = Expression.Parameter(typeof(object), "_unused");

    }
}