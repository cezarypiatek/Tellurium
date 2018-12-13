using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace Tellurium.MvcPages.WebPages.WebForms
{
    /// <summary>
    /// Strongly typed adapter for web form
    /// </summary>
    /// <typeparam name="TModel">Type of model connected with given web form</typeparam>
    public class MvcWebForm<TModel>: WebForm
    {
        public MvcWebForm(IWebElement webElement, RemoteWebDriver driver, List<IFormInputAdapter> supportedInputs, int numberOfSetRetries, AfterFieldValueSet afterFieldValueSet = AfterFieldValueSet.Nothing) 
            :base(webElement, driver, supportedInputs, numberOfSetRetries, afterFieldValueSet)
        {
        }

        public MvcWebForm()
        {
        }


        /// <summary>
        /// Set value for field indicated by expression
        /// </summary>
        /// <param name="field">Expression indicating given form field</param>
        /// <param name="value">Value to set for field</param>
        /// <param name="customAction">Action to perform after field value has been set</param>
        public void SetFieldValue<TFieldValue>(Expression<Func<TModel, TFieldValue>> field, string value, AfterFieldValueSet? customAction=null)
        {
            var fieldName = GetFieldName(field);
            base.SetFieldValue(fieldName, value, customAction);
        }

        /// <summary>
        /// Get value of field indicated by expression
        /// </summary>
        /// <param name="field">Expression indicating given form field</param>
        public string GetFieldValue<TFieldValue>(Expression<Func<TModel, TFieldValue>> field)
        {
            var fieldName = GetFieldName(field);
            return base.GetFieldValue(fieldName);
        }

        public FieldValueWatcher GetFieldValueWatcher<TFieldValue>(Expression<Func<TModel, TFieldValue>> field)
        {
            var fieldName = GetFieldName(field);
            return base.GetFieldValueWatcher(fieldName);
        }

        /// <summary>
        /// Perform action and wait until value of given field will change
        /// </summary>
        /// <param name="field">Field which value should change after given action</param>
        /// <param name="action">Action which should affect field value</param>
        public void AffectValueWith<TFieldValue>(Expression<Func<TModel, TFieldValue>> field, Action action)
        {
            var fieldName = GetFieldName(field);
            base.AffectValueWith(fieldName, action);
        }

        private static readonly Regex ArrayIndexerPattern = new Regex(@"\.?get_Item\(""?(.*?)""?\)", RegexOptions.Compiled);

        private static string GetFieldName<TFieldValue>(Expression<Func<TModel, TFieldValue>> field)
        {
            var text = field.Body.ToString();
            var firstDotPosition = text.IndexOf('.');
            if (firstDotPosition < 0)
            {
                return string.Empty;
            }
            text = text.Substring(firstDotPosition);
            text = ArrayIndexerPattern.Replace(text, "[$1]");
            return text.Trim('.');
        }
    }

    public enum AfterFieldValueSet
    {
        Nothing=1,
        Blur,
        MoveNext,
    }
}