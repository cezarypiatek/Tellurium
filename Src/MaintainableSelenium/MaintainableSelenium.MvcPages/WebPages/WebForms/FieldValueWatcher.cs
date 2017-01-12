using System;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;

namespace MaintainableSelenium.MvcPages.WebPages.WebForms
{
    public class FieldValueWatcher
    {
        private readonly RemoteWebDriver driver;
        private readonly WebFormField field;
        private string initialValue;

        public FieldValueWatcher(RemoteWebDriver driver, WebFormField field)
        {
            this.driver = driver;
            this.field = field;
            MemorizeValue();
        }

        public void MemorizeValue()
        {
            this.initialValue = this.field.GetValue();
        }

        public void WaitForValueChange()
        {
            var waiter = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            this.initialValue = waiter.Until(d =>
            {
                var currentValue = this.field.GetValue();
                if (this.initialValue != currentValue)
                {
                    return currentValue;
                }
                return null;
            });
        }
    }
}