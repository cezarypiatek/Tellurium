using System;
using MaintainableSelenium.MvcPages.SeleniumUtils;
using OpenQA.Selenium;

namespace MaintainableSelenium.MvcPages.WebPages.WebForms.DefaultInputAdapters
{
    public class HiddenFormInputAdapter : IFormInputAdapter
    {
        public bool CanHandle(IWebElement webElement)
        {
            return webElement.GetInputType() == "hidden";
        }

        public void SetValue(IWebElement webElement, string value)
        {
            throw new NotSupportedException("Setting value of hidden fields is not supported");
        }

        public string GetValue(IWebElement webElement)
        {
            throw new NotSupportedException("Getting value of hidden fields is not supported");
        }
    }
}