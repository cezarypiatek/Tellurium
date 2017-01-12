using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;
using MaintainableSelenium.MvcPages.SeleniumUtils;
using MaintainableSelenium.MvcPages.Utils;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

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
            var fieldWrapper = GetField(field);

            if (fieldWrapper.FieldAdapter.SupportSetRetry())
            {
                var success = RetryHelper.Retry(numberOfSetRetries, () =>
                {
                    fieldWrapper.SetValue(value);
                    return fieldWrapper.GetValue() == value;
                });

                if (success == false)
                {
                    var fieldName = GetFieldName(field);
                    throw new UnableToSetFieldValueException(fieldName, value);
                }
            }
            else
            {
                fieldWrapper.SetValue(value);
            }

            InvokeAfterFieldValueSet(fieldWrapper.FieldElement, customAction ?? afterFieldValueSet);
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
            var fieldWrapper = GetField(field);
            return fieldWrapper.GetValue();
        }

        private static string GetFieldName<TFieldValue>(Expression<Func<TModel, TFieldValue>> field)
        {
            return ExpressionHelper.GetExpressionText(field);
        }

        public FieldValueWatcher GetFieldValueWatcher<TFieldValue>(Expression<Func<TModel, TFieldValue>> field)
        {
            var fieldWrapper = GetField(field);
            return new FieldValueWatcher(Driver, fieldWrapper);
        }
        
        /// <summary>
        /// Perform action and wait until value of given field will change
        /// </summary>
        /// <param name="field">Field which value should change after given action</param>
        /// <param name="action">Action which should affect field value</param>
        public void AffectValueWith<TFieldValue>(Expression<Func<TModel, TFieldValue>> field, Action action)
        {
            var fieldValueWatcher = this.GetFieldValueWatcher(field);
            action();
            fieldValueWatcher.WaitForValueChange();
        }

        private WebFormField GetField<TFieldValue>(Expression<Func<TModel, TFieldValue>> field)
        {
            var fieldName = GetFieldName(field);
            return new WebFormField(WebElement, fieldName, SupportedInputs, Driver);
        }
    }

    public enum AfterFieldValueSet
    {
        Nothing=1,
        Blur,
        MoveNext,
    }
}