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

        public IStableWebElement FindFieldElement(IWebElement form)
        {
            return form.GetStableElementBy(By.Name(fieldName));
        }

        public string GetFieldDescription()
        {
            return $"Field with name '{fieldName}'";
        }
    }

    public interface IWebFormFieldLocator
    {
        IStableWebElement FindFieldElement(IWebElement form);
        string GetFieldDescription();
    }
}