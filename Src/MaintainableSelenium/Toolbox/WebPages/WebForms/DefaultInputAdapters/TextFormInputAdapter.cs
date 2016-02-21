using OpenQA.Selenium;

namespace MaintainableSelenium.Toolbox.WebPages.WebForms.DefaultInputAdapters
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
            webElement.SendKeys(value);
        }
    }
}