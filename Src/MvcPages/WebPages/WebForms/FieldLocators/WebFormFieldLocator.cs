using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using Tellurium.MvcPages.SeleniumUtils;

namespace Tellurium.MvcPages.WebPages.WebForms.FieldLocators
{
    public class ByFieldNameLocator:IWebFormFieldLocator
    {
        private readonly string fieldName;

        public ByFieldNameLocator(string fieldName)
        {
            this.fieldName = fieldName;
        }

        public IStableWebElement FindFieldElement(RemoteWebDriver driver, IWebElement form)
        {
            return driver.GetStableElementByInScope(form, By.Name(fieldName));
        }

        public string GetFieldDescription()
        {
            return $"Field with name '{fieldName}'";
        }
    }

    public interface IWebFormFieldLocator
    {
        IStableWebElement FindFieldElement(RemoteWebDriver driver, IWebElement form);
        string GetFieldDescription();
    }
}