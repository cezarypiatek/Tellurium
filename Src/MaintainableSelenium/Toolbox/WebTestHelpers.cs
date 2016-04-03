using System;
using System.Diagnostics.Contracts;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;

namespace MaintainableSelenium.Toolbox
{
    public static class WebTestHelpers
    {
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

        public static IWebElement GetElementById(this RemoteWebDriver driver, string formId)
        {
            return driver.GetElementBy(By.Id(formId));
        }

        public static IWebElement GetElementBy(this RemoteWebDriver driver, By by)
        {
            var waiter = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            var formElement = waiter.Until(ExpectedConditions.ElementIsVisible(by));
            return formElement;
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
        public static string GetInputType(this IWebElement inputElement)
        {
            var inputType = inputElement.GetAttribute("type") ?? string.Empty;
            return inputType.ToLower();
        }
    }
}