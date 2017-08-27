using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using Tellurium.MvcPages.SeleniumUtils;
using Tellurium.MvcPages.Utils;

namespace Tellurium.MvcPages.WebPages.WebForms.FieldLocators
{
    public class ByLabelTextLocator:IWebFormFieldLocator
    {
        private readonly string labelText;

        public ByLabelTextLocator(string labelText)
        {
            this.labelText = labelText;
        }

        public IStableWebElement FindFieldElement(RemoteWebDriver driver, IWebElement form)
        {
            var labelTextLiteral = XPathHelpers.ToXPathLiteral(labelText.Trim());
            var byLabelText = By.XPath($".//label[normalize-space(text()) = {labelTextLiteral}]");
            var labelElement = driver.GetStableElementByInScope(form, byLabelText);
            var inputId = labelElement.GetAttribute("for");
            if(string.IsNullOrWhiteSpace(inputId))
            {
                throw new ArgumentException($"Label with text '{labelText}' has empty 'for' attribute");
            }
            var byInputId = By.Id(inputId);
            return driver.GetStableElementByInScope(form, byInputId);
        }

        public string GetFieldDescription()
        {
            return $"Field with label {labelText}";
        }
    }
}