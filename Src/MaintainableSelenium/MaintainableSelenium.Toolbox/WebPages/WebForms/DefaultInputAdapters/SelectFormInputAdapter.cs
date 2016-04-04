using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace MaintainableSelenium.Toolbox.WebPages.WebForms.DefaultInputAdapters
{
    public class SelectFormInputAdapter : IFormInputAdapter
    {
        public bool CanHandle(IWebElement webElement)
        {
            return webElement.TagName.ToLower() == "select";
        }

        public void SetValue(IWebElement webElement, string value)
        {
            var select = new SelectElement(webElement);
            select.SelectByText(value);
        }
    }
}