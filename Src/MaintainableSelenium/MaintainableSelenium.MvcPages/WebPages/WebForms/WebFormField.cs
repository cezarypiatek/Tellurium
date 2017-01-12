using System;
using System.Collections.Generic;
using System.Linq;
using MaintainableSelenium.MvcPages.Utils;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;

namespace MaintainableSelenium.MvcPages.WebPages.WebForms
{
    public class WebFormField
    {
        private readonly IWebElement form;
        private readonly string fieldName;
        public IWebElement FieldElement { get; private set; }
        public IFormInputAdapter FieldAdapter { get; private set; }

        private readonly List<IFormInputAdapter> supportedImputAdapters;
        private readonly RemoteWebDriver driver;
        private static readonly TimeSpan InputSearchTimeout = TimeSpan.FromSeconds(30);
        

        public WebFormField(IWebElement form, string fieldName, List<IFormInputAdapter> supportedImputAdapters, RemoteWebDriver driver)
        {
            this.form = form;
            this.fieldName = fieldName;
            this.supportedImputAdapters = supportedImputAdapters;
            this.driver = driver;
            BuildFieldAccessFacility();
        }

        private IFormInputAdapter GetFieldAdapter(IWebElement fieldElement)
        {
            var input = supportedImputAdapters.FirstOrDefault(x => x.CanHandle(fieldElement));
            if (input == null)
            {
                throw new NotSupportedException("Not supported form element");
            }
            return input;
        }

        private IWebElement GetFieldElement()
        {
            var waiter = new WebDriverWait(driver, InputSearchTimeout);
            try
            {
                return waiter.Until(d => form.FindElement(By.Name(fieldName)));
            }
            catch (WebDriverTimeoutException)
            {
                throw new FieldNotFoundException(fieldName);
            }
        }

        public void SetValue(string value)
        {
            InvokeAccessor(() =>
            {
                FieldAdapter.SetValue(FieldElement, value);
            });
        }

        public string GetValue()
        {
            var result = string.Empty;
            InvokeAccessor(() =>
            {
                result = FieldAdapter.GetValue(FieldElement);
            });
            return result;
        }

        private void InvokeAccessor(Action accessor)
        {
            var success = RetryHelper.Retry(3, () =>
            {
                try
                {
                    accessor();
                    return true;
                }
                catch (StaleElementReferenceException)
                {
                    BuildFieldAccessFacility();
                    return false;
                }
            });

            if (success == false)
            {
                throw new FieldNotAccessibleException(fieldName);
            }
        }

        private void BuildFieldAccessFacility()
        {
            FieldElement = GetFieldElement();
            FieldAdapter = GetFieldAdapter(FieldElement);
        }
    }
}