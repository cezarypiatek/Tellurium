using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using MaintainableSelenium.Toolbox.WebPages;
using MaintainableSelenium.Toolbox.WebPages.WebForms;
using MaintainableSelenium.Toolbox.WebPages.WebForms.DefaultInputAdapters;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;

namespace MaintainableSelenium.Toolbox
{
    public static class WebTestHelpers
    {
        private static List<IFormInputAdapter> supportedInputsAdapters;

        static WebTestHelpers()
        {
            supportedInputsAdapters = new List<IFormInputAdapter>
            {
                new TextFormInputAdapter(),
                new SelectFormInputAdapter(),
                new CheckboxFormInputAdapter(),
                new RadioFormInputAdapter(),
                new HiddenFormInputAdapter()
            };
        }

        public static void SetSupportedInputs(List<IFormInputAdapter> inputsAdapters)
        {
            supportedInputsAdapters = inputsAdapters.ToList();
        }

        /// <summary>
        /// Get rid of focus from currently focused element
        /// </summary>
        /// <param name="driver">Selenium webdriver</param>
        public static void Blur(this RemoteWebDriver driver)
        {
            if(IsThereElementWithFocus(driver))
            {
                Thread.Sleep(500);
                driver.ExecuteScript("var f= document.querySelector(':focus'); if(f!=undefined){f.blur()}");
                Thread.Sleep(500);
            }
        }

        /// <summary>
        /// Check if any element has currently focus
        /// </summary>
        /// <param name="driver">Selenium webdriver</param>
        [Pure]
        private static bool IsThereElementWithFocus(RemoteWebDriver driver)
        {
            try
            {
                driver.FindElementByCssSelector(":focus");
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Return strongly typed adapter for web form with given id
        /// </summary>
        /// <typeparam name="TModel">Model connected with form</typeparam>
        /// <param name="driver">Selenium driver</param>
        /// <param name="formId">Id of expected form</param>
        public static WebForm<TModel> GetForm<TModel>(this RemoteWebDriver driver, string formId)
        {
            var formElement = GetElementById(driver, formId);
            return new WebForm<TModel>(formElement, driver, supportedInputsAdapters);
        }

        public static IWebElement GetElementById(this RemoteWebDriver driver, string formId)
        {
            return driver.GetElementBy(By.Id(formId));
        }

        /// <summary>
        /// Stop execution until element with given id appear
        /// </summary>
        /// <param name="driver">Selenium driver</param>
        /// <param name="elementId">Id of expected element</param>
        /// <param name="timeOut">Max time in seconds to wait</param>
        public static void WaitForElementWithId(this RemoteWebDriver driver, string elementId, int timeOut = 30)
        {
            var waiter = new WebDriverWait(driver, TimeSpan.FromSeconds(timeOut));
            waiter.Until(ExpectedConditions.ElementIsVisible(By.Id(elementId)));
        }
        
        /// <summary>
        /// Return page fragment with given id
        /// </summary>
        /// <param name="driver">Selenium driver</param>
        /// <param name="elementId">Id of expected element</param>
        public static PageFragment GetPageFragmentById(this RemoteWebDriver driver, string elementId)
        {
            var pageFragment = driver.GetElementBy(By.Id(elementId));
            return new PageFragment(driver, pageFragment);
        }

        private static IWebElement GetElementBy(this RemoteWebDriver driver, By by)
        {
            var waiter = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            var formElement = waiter.Until(ExpectedConditions.ElementIsVisible(by));
            return formElement;
        }

        /// <summary>
        /// Simulate click event on element with given id
        /// </summary>
        /// <param name="driver">Selenium driver</param>
        /// <param name="elementId">Id of expected element</param>
        public static void ClickOn(this RemoteWebDriver driver, string elementId)
        {
            driver.GetElementById(elementId).Click();
        }

        /// <summary>
        /// Return parent of given web element
        /// </summary>
        /// <param name="node">Child element</param>
        public static IWebElement GetParent(this IWebElement node)
        {
            return node.FindElement(By.XPath(".."));
        }

        /// <summary>
        /// Return type of input represented by the given web element
        /// </summary>
        /// <param name="inputElement">Web element</param>
        internal static string GetInputType(this IWebElement inputElement)
        {
            var inputType = inputElement.GetAttribute("type") ?? string.Empty;
            return inputType.ToLower();
        }
    }
}