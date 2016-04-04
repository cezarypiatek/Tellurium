using System;
using MaintainableSelenium.Toolbox.SeleniumUtils;
using OpenQA.Selenium;

namespace MaintainableSelenium.Toolbox.WebPages.WebForms.DefaultInputAdapters
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
    }
}