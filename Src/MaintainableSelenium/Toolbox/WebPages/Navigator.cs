using System;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Web.Mvc;
using OpenQA.Selenium.Remote;

namespace MaintainableSelenium.Toolbox.WebPages
{
    public interface INavigator
    {
        void NavigateTo<TController>(Expression<Action<TController>> action);
    }

    public class Navigator : INavigator
    {
        private readonly RemoteWebDriver driver;
        private readonly string rootUrl;

        public Navigator(RemoteWebDriver driver, string rootUrl)
        {
            this.driver = driver;
            this.rootUrl = rootUrl;
        }

        public void NavigateTo<TController>(Expression<Action<TController>> action)
        {
            var controllerName = GetControllerName<TController>();
            var actionName = GetMethodName(action);
            var areaName = GetAreaName<TController>();
            var actionLink = string.Format("{0}/{1}/{2}", areaName, controllerName, actionName).Trim('/');
            var url = string.Format("{0}/{1}", rootUrl.Trim('/'), actionLink);
            driver.Navigate().GoToUrl(url);
        }

        private string GetAreaName<TController>()
        {
            var controllerType = typeof(TController);
            var areaNameAttribute = controllerType.GetCustomAttribute<ActionLinkAreaAttribute>();
            if (areaNameAttribute == null)
            {
                return string.Empty;
            }
            return areaNameAttribute.Area;
        }

        private static string GetControllerName<TController>()
        {
            var controllerType = typeof (TController);
            return controllerType.Name.Replace("Controller", "");
        }

        private static string GetMethodName<T>(Expression<Action<T>> action)
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