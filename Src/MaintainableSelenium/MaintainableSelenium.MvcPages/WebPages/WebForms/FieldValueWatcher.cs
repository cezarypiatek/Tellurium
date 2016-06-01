using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;

namespace MaintainableSelenium.MvcPages.WebPages.WebForms
{
    public class FieldValueWatcher
    {
        private readonly RemoteWebDriver driver;
        private readonly IWebElement fieldElement;
        private readonly IFormInputAdapter fieldAdapter;
        private string initialValue;

        public FieldValueWatcher(RemoteWebDriver driver, IWebElement fieldElement, IFormInputAdapter fieldAdapter)
        {
            this.driver = driver;
            this.fieldElement = fieldElement;
            this.fieldAdapter = fieldAdapter;
            MemorizeValue();
        }

        public void MemorizeValue()
        {
            this.initialValue = this.fieldAdapter.GetValue(this.fieldElement);
        }

        public void WaitForValueChange()
        {
            var waiter = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            this.initialValue = waiter.Until(d =>
            {
                var currentValue = this.fieldAdapter.GetValue(fieldElement);
                if (this.initialValue != currentValue)
                {
                    return currentValue;
                }
                return null;
            });
        }
    }
}