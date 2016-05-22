using MaintainableSelenium.MvcPages.SeleniumUtils;
using OpenQA.Selenium;

namespace MaintainableSelenium.MvcPages.WebPages.WebForms.DefaultInputAdapters
{
    public class TextFormInputAdapter : IFormInputAdapter
    {
        public bool CanHandle(IWebElement webElement)
        {
            if (webElement.TagName.ToLower() == "textarea")
            {
                return true;
            }

            var inputType = webElement.GetInputType();
            return inputType == "text" || inputType == "password";
        }

        public void SetValue(IWebElement webElement, string value)
        {
            webElement.Clear();
            webElement.Type(value);
        }

        public string GetValue(IWebElement webElement)
        {
            return webElement.GetAttribute("value");
        }
    }
}