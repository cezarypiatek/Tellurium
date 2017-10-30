using System;
using System.Collections.Generic;
using System.Reflection;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;

namespace Tellurium.MvcPages.SeleniumUtils.WebDriverWrappers
{

    public static class WebDriverCommandExecutor
    {


        public static bool TryAddCommand(RemoteWebDriver driver, string commandName, CommandInfo commandInfo)
        {
            
            var commandExecutor = driver.GetType().GetProperty("CommandExecutor", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(driver) as ICommandExecutor;

            if (commandExecutor == null)
                throw new WebDriverException("Webdriver doesn't contain 'CommandExecutor' property.");

            return commandExecutor.CommandInfoRepository.TryAddCommand(commandName, commandInfo);

        }

        public static Response Execute(RemoteWebDriver driver, string driverCommandToExecute, Dictionary<string, object> parameters)
        {

            var executeMethod = driver.GetType().GetMethod("Execute", BindingFlags.Instance | BindingFlags.NonPublic);

            if (executeMethod == null)
                throw new WebDriverException("Webdriver doesn't contain 'Execute' method.");


            var response = executeMethod.Invoke(driver, new object[] { driverCommandToExecute, parameters }) as Response;

            if (response == null)
                throw new WebDriverException("Unexpected failure executing command; response was not in the proper format.");

            return response;
        }
    }
}
