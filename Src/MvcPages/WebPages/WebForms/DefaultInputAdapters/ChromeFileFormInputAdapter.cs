using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Remote;
using Tellurium.MvcPages.SeleniumUtils;
using Tellurium.MvcPages.SeleniumUtils.ChromeRemoteInterface;

namespace Tellurium.MvcPages.WebPages.WebForms.DefaultInputAdapters
{
    public class ChromeFileFormInputAdapter:IFormInputAdapter
    {
        private ChromeRemoteInterface chromeremoteInterface;

        public bool CanHandle(IWebElement webElement)
        {
            return webElement.GetInputType() == "file" &&  webElement is IWrapsDriver;
        }

        public void SetValue(IWebElement webElement, string value)
        {
            if (chromeremoteInterface == null)
            {
                var remoteWebDriver = ((IWrapsDriver)webElement).WrappedDriver as RemoteWebDriver;
                chromeremoteInterface = new ChromeRemoteInterface(remoteWebDriver);
            }
            chromeremoteInterface.SetFileInputFiles(webElement, new []{value});
        }

        public string GetValue(IWebElement webElement)
        {
            return webElement.GetAttribute("value");
        }

        public bool SupportSetRetry()
        {
            return false;
        }
    }
}