using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using Tellurium.MvcPages.SeleniumUtils;
using Tellurium.MvcPages.Utils;

namespace Tellurium.MvcPages.WebPages.WebForms
{
    /// <summary>
    /// Weakly typed adapter for web form
    /// </summary>
    public class WebForm : PageFragment
    {
        private readonly int numberOfSetRetries;
        private readonly AfterFieldValueSet afterFieldValueSet;
        private List<IFormInputAdapter> SupportedInputs { get; set; }

        public WebForm(IWebElement webElement, RemoteWebDriver driver, List<IFormInputAdapter> supportedInputs, int numberOfSetRetries, AfterFieldValueSet afterFieldValueSet = AfterFieldValueSet.Nothing) : base(driver, webElement)
        {
            this.numberOfSetRetries = numberOfSetRetries;
            this.afterFieldValueSet = afterFieldValueSet;
            SupportedInputs = supportedInputs;
        }

        /// <summary>
        /// Set value for field indicated by field name
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="value">Value to set for field</param>
        /// <param name="customAction">Action to perform after field value has been set</param>
        public void SetFieldValue(string fieldName, string value, AfterFieldValueSet? customAction = null)
        {
            var fieldWrapper = GetFieldWrapper(fieldName);

            if (fieldWrapper.FieldAdapter.SupportSetRetry())
            {
                var success = RetryHelper.Retry(numberOfSetRetries, () =>
                {
                    fieldWrapper.SetValue(value);
                    return fieldWrapper.GetValue() == value;
                });

                if (success == false)
                {
                    throw new UnableToSetFieldValueException(fieldName, value);
                }
            }
            else
            {
                fieldWrapper.SetValue(value);
            }

            InvokeAfterFieldValueSet(fieldWrapper.FieldElement, customAction ?? afterFieldValueSet);
        }

        /// <summary>
        /// Get value of field indicated by field name
        /// </summary>
        /// <param name="fieldName">Field name</param>
        public string GetFieldValue(string fieldName)
        {
            var fieldWrapper = GetFieldWrapper(fieldName);
            return fieldWrapper.GetValue();
        }

        public FieldValueWatcher GetFieldValueWatcher(string fieldName)
        {
            var fieldWrapper = GetFieldWrapper(fieldName);
            return new FieldValueWatcher(Driver, fieldWrapper);
        }

        /// <summary>
        /// Perform action and wait until value of given field will change
        /// </summary>
        /// <param name="fieldName">Field which value should change after given action</param>
        /// <param name="action">Action which should affect field value</param>
        public void AffectValueWith(string fieldName, Action action)
        {
            var fieldValueWatcher = this.GetFieldValueWatcher(fieldName);
            action();
            fieldValueWatcher.WaitForValueChange();
        }
        
        private void InvokeAfterFieldValueSet(IWebElement fieldElement, AfterFieldValueSet actionType)
        {
            switch (actionType)
            {
                case AfterFieldValueSet.Nothing:
                    break;
                case AfterFieldValueSet.Blur:
                    Driver.Blur();
                    break;
                case AfterFieldValueSet.MoveNext:
                    fieldElement.SendKeys(Keys.Tab);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private WebFormField GetFieldWrapper(string fieldName)
        {
            return new WebFormField(WebElement, fieldName, SupportedInputs, Driver);
        }
    }
}