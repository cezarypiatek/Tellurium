using System;
using System.Collections.ObjectModel;
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
        /// <param name="timeout">Timeout for element search</param>
        public static IStableWebElement GetStableAccessibleElementById(this RemoteWebDriver driver, string elementId, int timeout = SearchElementDefaultTimeout)
        {
            By @by = By.Id(elementId);
            return GetStableAccessibleElementByInScope(driver, @by, driver, timeout);
        }

        internal static IStableWebElement GetStableAccessibleElementByInScope(this RemoteWebDriver driver, By @by, ISearchContext scope, int timeout = SearchElementDefaultTimeout)
        {
            var foundElement = GetFirstAccessibleElement(driver, @by, scope, timeout);
            return new StableWebElement(scope, foundElement, @by, SearchApproachType.FirstAccessible);
        }

        

        private static IWebElement GetFirstAccessibleElement(RemoteWebDriver driver, By by, ISearchContext scope, int timeout)
        {
            ReadOnlyCollection<IWebElement> candidates = null;
            try
            {
                return driver.WaitUntil(timeout, (a) =>
                {
                    candidates = scope.FindElements(by);
                    return candidates.FirstAccessibleOrDefault();
                });
            }
            catch (WebDriverTimeoutException ex)
            {
                throw new CannotFindAccessibleElementByException(by, scope, candidates,  ex);
            }
        }

        private static bool IsElementAccessible(this IWebElement element)
        {
            return element != null && element.Displayed && element.Enabled;
        }

        private static IWebElement FirstAccessibleOrDefault(this ReadOnlyCollection<IWebElement> elements)
        {
            return elements.FirstOrDefault(element =>
            {
                try
                {
                    return element.IsElementAccessible();
                }
                catch (StaleElementReferenceException)
                {
                    return false;
                }
            });
        }

        public static TResult WaitUntil<TResult>(this RemoteWebDriver driver, int timeout, Func<IWebDriver, TResult> waitPredictor)
        {
            var waiter = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
            return waiter.Until(waitPredictor);
        }

        internal static IStableWebElement GetStableElementWithText(this RemoteWebDriver driver, ISearchContext scope, string text, bool isPartialText)
        {
            var by = BuildLocatorByContainedText(text, isPartialText);
            return GetStableAccessibleElementByInScope(driver, by, scope);
        }

        private static By BuildLocatorByContainedText(string text, bool isPartialText)
        {
            return isPartialText ? ByText.FromPartial(text) : ByText.From(text);
        }

        internal static IWebElement TryFindElement(this ISearchContext context, By by)
        {
            return ExceptionHelper.SwallowException(() => context.FindElement(@by), null);
        }
    }
}