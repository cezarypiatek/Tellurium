using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using Tellurium.MvcPages.SeleniumUtils;
using Tellurium.MvcPages.Utils;
using Tellurium.MvcPages.WebPages.WebForms.FieldLocators;

namespace Tellurium.MvcPages.WebPages.WebForms
{
    /// <summary>
    /// Weakly typed adapter for web form
    /// </summary>
    public class WebForm : PageFragment
    {
        private int numberOfSetRetries;
        private AfterFieldValueSet afterFieldValueSet;
        private List<IFormInputAdapter> SupportedInputs { get; set; }

        public WebForm(IWebElement webElement, RemoteWebDriver driver, List<IFormInputAdapter> supportedInputs, int numberOfSetRetries, AfterFieldValueSet afterFieldValueSet = AfterFieldValueSet.Nothing) : base(driver, webElement)
        {
            this.numberOfSetRetries = numberOfSetRetries;
            this.afterFieldValueSet = afterFieldValueSet;
            SupportedInputs = supportedInputs;
        }

        public WebForm()
        {
        }

        public void Init(IWebElement webElement, RemoteWebDriver driver, List<IFormInputAdapter> supportedInputs, int numberOfSetRetries, AfterFieldValueSet afterFieldValueSet = AfterFieldValueSet.Nothing)
        {
            this.numberOfSetRetries = numberOfSetRetries;
            this.afterFieldValueSet = afterFieldValueSet;
            SupportedInputs = supportedInputs;
            base.Init(driver, webElement);
        }

        /// <summary>
        /// Set value for field indicated by field name
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="value">Value to set for field</param>
        /// <param name="customAction">Action to perform after field value has been set</param>
        public void SetFieldValue(string fieldName, string value, AfterFieldValueSet? customAction = null)
        {
            var fieldLocator = new ByFieldNameLocator(fieldName);
            SetFieldValue(fieldLocator, value, customAction);
        }
        
        /// <summary>
        /// Set value for field with given label's text
        /// </summary>
        /// <param name="labelText">Label text</param>
        /// <param name="value">Value to set for field</param>
        /// <param name="customAction">Action to perform after field value has been set</param>
        public void SetFieldValueByLabel(string labelText, string value, AfterFieldValueSet? customAction = null)
        {
            var fieldLocator = new ByLabelTextLocator(labelText);
            SetFieldValue(fieldLocator, value, customAction);
        }

        private void SetFieldValue(IWebFormFieldLocator fieldLocator, string value, AfterFieldValueSet? customAction)
        {
            var fieldWrapper = CreateFieldWrapper(fieldLocator);

            var retryResult = RetryHelper.RetryWithExceptions(numberOfSetRetries, () =>
            {
                fieldWrapper.SetValue(value);
                if (fieldWrapper.FieldAdapter.SupportSetRetry())
                {
                    return fieldWrapper.GetValue() == value;
                }
                return true;
            });

            if (retryResult.Success == false)
            {
                throw new UnableToSetFieldValueException(fieldLocator.GetFieldDescription(), value, retryResult.LastException);
            }

            InvokeAfterFieldValueSet(fieldWrapper.FieldElement, customAction ?? afterFieldValueSet);
        }

        /// <summary>
        /// Get value of field with given label's text
        /// </summary>
        /// <param name="labelText">Field name</param>
        public string GetFieldValue(string labelText)
        {
            var fieldLocator = new ByFieldNameLocator(labelText);
            return GetFieldValue(fieldLocator);
        }
        
        /// <summary>
        /// Get value of field indicated by field name
        /// </summary>
        /// <param name="fieldName">Field name</param>
        public string GetFieldValueByLabel(string fieldName)
        {
            var fieldLocator = new ByLabelTextLocator(fieldName);
            return GetFieldValue(fieldLocator);
        }

        private string GetFieldValue(IWebFormFieldLocator fieldLocator)
        {
            var fieldWrapper = CreateFieldWrapper(fieldLocator);
            return fieldWrapper.GetValue();
        }

        public FieldValueWatcher GetFieldValueWatcher(string fieldName)
        {
            var fieldLocator = new ByFieldNameLocator(fieldName);
            var fieldWrapper = CreateFieldWrapper(fieldLocator);
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

        private WebFormField CreateFieldWrapper(IWebFormFieldLocator fieldLocator)
        {
            return new WebFormField(WebElement, fieldLocator, SupportedInputs, Driver);
        }
    }
}