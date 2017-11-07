using System;
using System.Collections.Generic;
using OpenQA.Selenium.Remote;

namespace Tellurium.MvcPages.SeleniumUtils.ChromeRemoteInterface
{
    internal static class RemoteInterfaceResponseExtensions
    {
        public static T GetValue<T>(this Response response, string valuePath)
        {
            var root = response.Value;
            foreach (var key in valuePath.Split('.'))
            {
                if ((root as Dictionary<string, object>)?.TryGetValue(key, out root) == false)
                {
                    throw new ArgumentException($"The path '{valuePath}' is not vallid. Cannot expand '{key}' part.");
                }
                
            }
            return (T) root;
        }
    }
}