using OpenQA.Selenium;

namespace Tellurium.MvcPages.WebPages.WebForms
{
    public interface IFormInputAdapter
    {
        bool CanHandle(IWebElement webElement);
        void SetValue(IWebElement webElement, string value);
        string GetValue(IWebElement webElement);
        bool SupportSetRetry();
    }
}