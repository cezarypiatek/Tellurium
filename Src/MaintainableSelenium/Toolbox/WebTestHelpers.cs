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

        public static void Blur(this RemoteWebDriver driver)
        {
            if(IsThereElementWithFocus(driver))
            {
                Thread.Sleep(500);
                driver.ExecuteScript("var f= document.querySelector(':focus'); if(f!=undefined){f.blur()}");
                Thread.Sleep(500);
            }
        }

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

        public static WebForm<TModel> GetForm<TModel>(this RemoteWebDriver driver, string formId)
        {
            var formElement = GetElementById(driver, formId);
            return new WebForm<TModel>(formElement, driver, supportedInputsAdapters);
        }

        public static IWebElement GetElementById(this RemoteWebDriver driver, string formId)
        {
            return driver.GetElementBy(By.Id(formId));
        }
        
        public static PageFragment GetPageFragmentById(this RemoteWebDriver driver, string formId)
        {
            var pageFragment = driver.GetElementBy(By.Id(formId));
            return new PageFragment(driver, pageFragment);
        }

        private static IWebElement GetElementBy(this RemoteWebDriver driver, By by)
        {
            var waiter = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            var formElement = waiter.Until(ExpectedConditions.ElementIsVisible(by));
            return formElement;
        }

        public static void ClickOn(this RemoteWebDriver driver, string elementId)
        {
            driver.GetElementById(elementId).Click();
        }

        public static IWebElement GetParent(this IWebElement node)
        {
            return node.FindElement(By.XPath(".."));
        }

        internal static string GetInputType(this IWebElement inputElement)
        {
            var inputType = inputElement.GetAttribute("type") ?? string.Empty;
            return inputType.ToLower();
        }
    }
}