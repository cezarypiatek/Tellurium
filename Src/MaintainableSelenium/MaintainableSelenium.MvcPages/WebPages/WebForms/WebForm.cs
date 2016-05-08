using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
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
        private static TimeSpan InputSearchTimeout = TimeSpan.FromSeconds(30);

        private List<IFormInputAdapter> SupportedInputs { get; set; }


        public WebForm(IWebElement webElement, RemoteWebDriver driver, List<IFormInputAdapter> supportedInputs) : base(driver, webElement)
        {
            SupportedInputs = supportedInputs;
        }

        /// <summary>
        /// Set value for field indicated by expression
        /// </summary>
        /// <param name="field">Expression indicating given form field</param>
        /// <param name="value">Value to set for fields</param>
        public void SetFieldValue<TFieldValue>(Expression<Func<TModel, TFieldValue>> field, string value)
        {
            var fieldElement = GetField(field);
            var fieldAdapter = GetFieldAdapter(fieldElement);
            fieldAdapter.SetValue(fieldElement, value);
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
            var fieldName = ExpressionHelper.GetExpressionText(field);
            var waiter = new WebDriverWait(Driver, InputSearchTimeout);
            try
            {
                return waiter.Until(d => WebElement.FindElement(By.Name(fieldName)));
            }
            catch (TimeoutException)
            {
                throw new NoWebForFieldException(fieldName);
            }
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
    }
}