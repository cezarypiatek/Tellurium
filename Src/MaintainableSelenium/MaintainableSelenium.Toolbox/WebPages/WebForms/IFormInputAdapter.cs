using OpenQA.Selenium;

namespace MaintainableSelenium.Toolbox.WebPages.WebForms
{
    public interface IFormInputAdapter
    {
        bool CanHandle(IWebElement webElement);
        void SetValue(IWebElement webElement, string value);
    }
}