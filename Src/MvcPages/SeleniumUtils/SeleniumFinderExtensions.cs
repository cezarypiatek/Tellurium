using System;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using Tellurium.MvcPages.SeleniumUtils.Exceptions;
using Tellurium.MvcPages.Utils;

namespace Tellurium.MvcPages.SeleniumUtils
{
    public static class SeleniumFinderExtensions
    {
        internal const int SearchElementDefaultTimeout = 30;

        /// <summary>
        /// Search for element with given id
        /// </summary>
        /// <param name="driver">Selenium driver</param>
        /// <param name="elementId">Id of expected element</param>
        /// <param name="timeout">Timout for element serch</param>
        public static IStableWebElement GetStableAccessibleElementById(this RemoteWebDriver driver, string elementId, int timeout = SearchElementDefaultTimeout)
        {
            By @by = By.Id(elementId);
            return GetStableAccessibleElementByInScope(driver, @by, driver, timeout);
        }

        internal static IStableWebElement GetStableAccessibleElementByInScope(this RemoteWebDriver driver, By @by, ISearchContext scope, int timeout = SearchElementDefaultTimeout)
        {
            var foundElement = GetFirstAccessibleElement(driver, @by, scope, timeout);
            return new StableWebElement(scope, foundElement, @by);
        }

        private static IWebElement GetFirstAccessibleElement(RemoteWebDriver driver, By @by, ISearchContext scope, int timeout)
        {
            try
            {
                return driver.WaitUntil(timeout, (a) =>
                {
                    var elements = scope.FindElements(@by);

                    try
                    {
                        return elements.FirstOrDefault(element => element != null && element.Displayed && element.Enabled);
                    }
                    catch (StaleElementReferenceException)
                    {
                        return null;
                    }
                });
            }
            catch (WebDriverTimeoutException ex)
            {
                throw new CannotFindElementByException(@by, ex);   
            }
        }

        public static IStableWebElement GetStableElementById(this RemoteWebDriver driver, string elementId, int timeout = SearchElementDefaultTimeout)
        {
            By @by = By.Id(elementId);
            return GetStableElementByInScope(driver, driver, @by, timeout);
        }

        public static IStableWebElement GetStableElementByInScope(this RemoteWebDriver driver, ISearchContext scope, By by, int timeout = 30)
        {
            var foundElement = GetFirstElement(driver, scope, @by, timeout);
            return new StableWebElement(scope,foundElement,@by);
        }

        private static IWebElement GetFirstElement(RemoteWebDriver driver, ISearchContext scope, By @by, int timeout)
        {
            try
            {
                return driver.WaitUntil(timeout, d => scope.FindElement(@by));
            }
            catch (WebDriverTimeoutException ex)
            {
                throw new CannotFindElementByException(@by, ex);
            }
        }

        public static TResult WaitUntil<TResult>(this RemoteWebDriver driver, int timeout, Func<IWebDriver, TResult> waitPredictor)
        {
            var waiter = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
            return waiter.Until(waitPredictor);
        }

        internal static IStableWebElement GetStableElementWithText(this RemoteWebDriver driver, ISearchContext scope, string text, bool isPartialText)
        {
            try
            {
                var by = BuildLocatorByContainedText(text, isPartialText);
                return GetStableAccessibleElementByInScope(driver, @by, scope);
            }
            catch (CannotFindElementByException ex)
            {
                throw new WebElementNotFoundException($"Cannot find element with text='{text}'", ex);
            }
        }

        private static By BuildLocatorByContainedText(string text, bool isPartialText)
        {
            var xpathLiteral = XPathHelpers.ToXPathLiteral(text.Trim());
            var by = isPartialText
                ? By.XPath(string.Format(".//*[contains(text(), {0}) or ((@type='submit' or  @type='reset') and contains(@value,{0})) or contains(@title,{0})]", xpathLiteral))
                : By.XPath(string.Format(".//*[((normalize-space(.) = {0}) and (count(*)=0) ) or (normalize-space(text()) = {0}) or ((@type='submit' or  @type='reset') and @value={0}) or (@title={0})]",xpathLiteral));
            return by;
        }

        internal static IWebElement FindElementBy(this ISearchContext context, By by)
        {
            return ExceptionHelper.SwallowException(() => context.FindElement(@by), null);
        }
    }
}