using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using MaintainableSelenium.MvcPages.SeleniumUtils;
using MaintainableSelenium.MvcPages.Utils;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;

namespace MaintainableSelenium.MvcPages.WebPages.WebForms
{
    /// <summary>
    /// Strongly typed adapter for web form
    /// </summary>
    /// <typeparam name="TModel">Type of model connected with given web form</typeparam>
    public class WebForm<TModel>: PageFragment
    {
        private readonly int numberOfSetRetries;
        private readonly AfterFieldValueSet afterFieldValueSet;
        private static TimeSpan InputSearchTimeout = TimeSpan.FromSeconds(30);

        private List<IFormInputAdapter> SupportedInputs { get; set; }

        public WebForm(IWebElement webElement, RemoteWebDriver driver, List<IFormInputAdapter> supportedInputs, int numberOfSetRetries, AfterFieldValueSet afterFieldValueSet = AfterFieldValueSet.Nothing) 
            : base(driver, webElement)
        {
            this.numberOfSetRetries = numberOfSetRetries;
            this.afterFieldValueSet = afterFieldValueSet;
            SupportedInputs = supportedInputs;
        }

        /// <summary>
        /// Set value for field indicated by expression
        /// </summary>
        /// <param name="field">Expression indicating given form field</param>
        /// <param name="value">Value to set for fields</param>
        /// <param name="customAction">Action to perform after field value has been set</param>
        public void SetFieldValue<TFieldValue>(Expression<Func<TModel, TFieldValue>> field, string value, AfterFieldValueSet? customAction=null)
        {
            var fieldElement = GetField(field);
            var fieldAdapter = GetFieldAdapter(fieldElement);

            if (fieldAdapter.SupportSetRetry())
            {
                var success = RetryHelper.Retry(numberOfSetRetries, () =>
                {
                    fieldAdapter.SetValue(fieldElement, value);
                    return fieldAdapter.GetValue(fieldElement) == value;
                });

                if (success == false)
                {
                    var fieldName = GetFieldName(field);
                    throw new UnableToSetFieldValueException(fieldName, value);
                }
            }
            else
            {
                fieldAdapter.SetValue(fieldElement, value);
            }

            InvokeAfterFieldValueSet(fieldElement, customAction ?? afterFieldValueSet);
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

        /// <summary>
        /// Get value of field indicated by expression
        /// </summary>
        /// <param name="field">Expression indicating given form field</param>
        public string GetFieldValue<TFieldValue>(Expression<Func<TModel, TFieldValue>> field)
        {
            var fieldElement = GetField(field);
            var fieldAdapter = GetFieldAdapter(fieldElement);
            return fieldAdapter.GetValue(fieldElement);
        }

        private IWebElement GetField<TFieldValue>(Expression<Func<TModel, TFieldValue>> field)
        {
            var fieldName = GetFieldName(field);
            var waiter = new WebDriverWait(Driver, InputSearchTimeout);
            try
            {
                return waiter.Until(d => WebElement.FindElement(By.Name(fieldName)));
            }
            catch (WebDriverTimeoutException)
            {
                throw new FieldNotFoundException(fieldName);
            }
        }

        private static string GetFieldName<TFieldValue>(Expression<Func<TModel, TFieldValue>> field)
        {
            return ExpressionHelper.GetExpressionText(field);
        }

        private IFormInputAdapter GetFieldAdapter(IWebElement fieldElement)
        {
            var input = SupportedInputs.FirstOrDefault(x => x.CanHandle(fieldElement));
            if (input == null)
            {
                throw new NotSupportedException("Not supported form element");
            }
            return input;
        }

        public FieldValueWatcher GetFieldValueWatcher<TFieldValue>(Expression<Func<TModel, TFieldValue>> field)
        {
            var fieldElement = GetField(field);
            var fieldAdapter = GetFieldAdapter(fieldElement);
            return new FieldValueWatcher(Driver, fieldElement, fieldAdapter);
        }
    }

    public enum AfterFieldValueSet
    {
        Nothing=1,
        Blur,
        MoveNext,
    }
}