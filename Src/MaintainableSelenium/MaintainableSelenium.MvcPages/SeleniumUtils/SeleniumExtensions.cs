using System;
using System.Diagnostics.Contracts;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;

namespace MaintainableSelenium.MvcPages.SeleniumUtils
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
        /// Tyoe text into field
        /// </summary>
        /// <param name="input">Field</param>
        /// <param name="text">Text to tyoe</param>
        /// <param name="speed">Speed of typing (chars per minute). 0 means default selenium speed</param>
        public static void Type(this IWebElement input, string text, int speed = 0)
        {
            input.Focus();

            if (speed == 0)
            {
                input.SendKeys(text);
            }
            else
            {
                var delay = (1000*60)/speed;
                foreach (var charToType in text)
                {
                    input.SendKeys(charToType.ToString());
                    Thread.Sleep(delay);
                }
            }
        }

        public static void Focus(this IWebElement input)
        {
            input.SendKeys("");
        }

        public static int GetVerticalScrollWidth(this RemoteWebDriver driver)
        {
            //INFO: It's hard to get scrollbar width using JS. 17 its default size of scrollbar on Ms Windows platform
            return 17;
        }

        public static int GetWindowHeight(this RemoteWebDriver driver)
        {
            return (int)(long)driver.ExecuteScript("return window.innerHeight");
        } 
        
        internal static void WaitForContentChange(this RemoteWebDriver driver, string containerId, int  timeout = SearchElementDefaultTimeout)
        {
            driver.ExecuteScript(@"var $$expectedId = arguments[0];
__selenium_observers__ =  window.__selenium_observers__ || {};
(function(){		
		var target = document.getElementById($$expectedId);
		__selenium_observers__[$$expectedId] = {
				observer: new MutationObserver(function(mutations) {
					__selenium_observers__[$$expectedId].occured = true;
					__selenium_observers__[$$expectedId].observer.disconnect();
				}),
				occured:false
		};
		var config = { attributes: true, childList: true, characterData: true, subtree: true };

		__selenium_observers__[$$expectedId].observer.observe(target, config);
})();", containerId);
            var waiter = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
            waiter.Until(d => (bool)driver.ExecuteScript("return __selenium_observers__[arguments[0]].occured;",containerId));
        }

        public static int GetPageHeight(this RemoteWebDriver driver)
        {
            var scriptResult = driver.ExecuteScript("return Math.max(document.body.scrollHeight, document.body.offsetHeight, document.documentElement.clientHeight, document.documentElement.scrollHeight, document.documentElement.offsetHeight);");
            return (int)(long)scriptResult;
        }

        public static void ScrollTo(this RemoteWebDriver driver, int y)
        {
            driver.ExecuteScript(string.Format("window.scrollTo(0,{0})", y));
            Thread.Sleep(100);
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
            try
            {
                return driver.GetElementBy(By.Id(elementId), timeout);
            }
            catch (WebDriverTimeoutException ex)
            {
                if (ex.InnerException is NoSuchElementException)
                {
                    var message = string.Format("Cannot find element with id='{0}'", elementId);
                    throw new WebElementNotFoundException(message, ex);
                }
                throw;
            }
        }

        /// <summary>
        /// Search for element using <see cref="By"/> criterion
        /// </summary>
        /// <param name="driver">Selenium driver</param>
        /// <param name="by"><see cref="By"/> criterion for given element</param>
        /// <param name="timeout">Timout for element serch</param>
        private static IWebElement GetElementBy(this RemoteWebDriver driver, By by, int timeout = SearchElementDefaultTimeout)
        {
            var waiter = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
            var expectedElement = waiter.Until(
                (a) =>
                {
                    try
                    {
                        var element = driver.FindElement(by);
                        if (element != null && element.Displayed && element.Enabled)
                        {
                            return element;
                        }
                        return null;
                    }
                    catch(StaleElementReferenceException)
                    {
                        return null;
                    }
                });
            return expectedElement;
        }

        private static IWebElement GetElementByInScope(RemoteWebDriver driver, By @by, IWebElement scope, int timeout = SearchElementDefaultTimeout)
        {
            var waiter = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
            var expectedElement = waiter.Until(
                (a) =>
                {
                    try
                    {
                        var element = scope.FindElement(@by);
                        if (element != null && element.Displayed && element.Enabled)
                        {
                            return element;
                        }
                        return null;
                    }
                    catch (StaleElementReferenceException)
                    {
                        return null;
                    }
                });
            return expectedElement;
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
        /// Click on any element with given text
        /// </summary>
        /// <param name="driver">Selenium driver</param>
        /// <param name="scope">Scope element to narrow link search</param>
        /// <param name="linkText">Element tekst</param>
        public static  void ClickOnElementWithText(this RemoteWebDriver driver, IWebElement scope, string linkText)
        {
            try
            {
                var by = By.XPath(string.Format(".//*[contains(text(), '{0}') or (@type='submit' and @value='{0}')]", linkText));
                var expectedElement = GetElementByInScope(driver, @by, scope);
                expectedElement.Click();
            }
            catch (WebDriverTimeoutException ex)
            {
                if (ex.InnerException is NoSuchElementException)
                {
                    var message = string.Format("Cannot find element with text='{0}'", linkText);
                    throw new WebElementNotFoundException(message, ex);
                }
                throw;
            }
        }
    }
}