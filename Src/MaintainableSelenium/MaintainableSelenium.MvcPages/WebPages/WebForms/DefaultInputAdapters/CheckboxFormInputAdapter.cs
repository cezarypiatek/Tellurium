using System;
using MaintainableSelenium.MvcPages.SeleniumUtils;
using OpenQA.Selenium;

namespace MaintainableSelenium.MvcPages.WebPages.WebForms.DefaultInputAdapters
{
    public class CheckboxFormInputAdapter : IFormInputAdapter
    {
        public const string Checked = "true";
        public const string UnChecked = "false";
        
        public bool CanHandle(IWebElement webElement)
        {
            return webElement.GetInputType() == "checkbox";
        }

        public void SetValue(IWebElement webElement, string value)
        {
            if (webElement.Selected != Boolean.Parse(value))
            {
                webElement.Click();
            }
        }

        public string GetValue(IWebElement webElement)
        {
            return webElement.Selected ? Checked : UnChecked;
        }

        public bool SupportSetRetry()
        {
            return true;
        }
    }
}