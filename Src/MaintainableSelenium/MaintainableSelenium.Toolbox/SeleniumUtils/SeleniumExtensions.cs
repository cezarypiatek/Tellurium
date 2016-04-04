using System;
using System.Diagnostics.Contracts;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;

namespace MaintainableSelenium.Toolbox.SeleniumUtils
{
    public static class SeleniumExtensions
    {
        private const int SearchElementDefaultTimeout = 30;

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
        /// Search for element with given id
        /// </summary>
        /// <param name="driver">Selenium driver</param>
        /// <param name="elementId">Id of expected element</param>
        /// <param name="timeout">Timout for element serch</param>
        public static IWebElement GetElementById(this RemoteWebDriver driver, string elementId, int timeout = SearchElementDefaultTimeout)
        {
            return driver.GetElementBy(By.Id(elementId), timeout);
        }

        /// <summary>
        /// Search for element using <see cref="By"/> criterion
        /// </summary>
        /// <param name="driver">Selenium driver</param>
        /// <param name="by"><see cref="By"/> criterion for given element</param>
        /// <param name="timeout">Timout for element serch</param>
        public static IWebElement GetElementBy(this RemoteWebDriver driver, By by, int timeout = SearchElementDefaultTimeout)
        {
            var waiter = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
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

        /// <summary>
        /// Click on link with given text
        /// </summary>
        /// <param name="driver">Selenium driver</param>
        /// <param name="scope">Scope element to narrow link search</param>
        /// <param name="linkText">Link tekst</param>
        public static  void ClickOnLinkWithLabel(this RemoteWebDriver driver, IWebElement scope, string linkText)
        {
            var by = By.PartialLinkText(linkText);
            ClickOn(driver, scope, @by);
        }

        /// <summary>
        /// Click on any element with given text
        /// </summary>
        /// <param name="driver">Selenium driver</param>
        /// <param name="scope">Scope element to narrow link search</param>
        /// <param name="linkText">Element tekst</param>
        public static  void ClickOnElementWithLabel(this RemoteWebDriver driver, IWebElement scope, string linkText)
        {
            var by = By.XPath(string.Format("//*[contains(text(), '{0}')]", linkText));
            ClickOn(driver, scope, @by);
        }

        private static void ClickOn(this RemoteWebDriver driver, IWebElement scope,  By @by)
        {
            var waiter = new WebDriverWait(driver, TimeSpan.FromSeconds(SearchElementDefaultTimeout));
            var linkElement = waiter.Until(
                (a) =>
                {
                    var link = scope.FindElement(@by);
                    if (link != null && link.Displayed && link.Enabled)
                    {
                        return link;
                    }
                    return null;
                });
            linkElement.Click();
        }
    }
}