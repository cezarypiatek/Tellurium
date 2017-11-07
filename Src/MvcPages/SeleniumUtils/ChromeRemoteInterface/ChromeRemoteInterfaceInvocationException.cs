using System;
using OpenQA.Selenium.Remote;

namespace Tellurium.MvcPages.SeleniumUtils.ChromeRemoteInterface
{
    public class ChromeRemoteInterfaceInvocationException:Exception
    {
        public string Operation { get; }
        public object Parameters { get; }
        public Response Response { get; }

        public ChromeRemoteInterfaceInvocationException(string operation, object parameters, Response response)
            :base($"Invocation of '{operation}' failed. Response status {response.Status}")
        {
            Operation = operation;
            Parameters = parameters;
            Response = response;
        }
    }
}