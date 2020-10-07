using System;
using System.Collections.ObjectModel;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using Tellurium.MvcPages.SeleniumUtils.Exceptions;

namespace Tellurium.MvcPages.SeleniumUtils
{

    public static class PublicExtensions
    {

        public static IStableWebElement FindStableElement(this ISearchContext context, By by)
        {
            var element = context.FindElement(by);
            return new StableWebElement(context, element, by, SearchApproachType.First);
        }

        public static IStableWebElement TryFindStableElement(this ISearchContext context, By by)
        {
            var element = context.TryFindElement(by);
            if (element == null)
            {
                return null;
            }
            return new StableWebElement(context, element, by, SearchApproachType.First);
        }

        public static IStableWebElement GetStableElementById(this RemoteWebDriver driver, string elementId, int timeout = 30)
        {
            By @by = By.Id(elementId);
            return GetStableElementByInScope(driver, driver, @by, timeout);
        }

        public static IStableWebElement GetStableElementByInScope(this RemoteWebDriver driver, ISearchContext scope, By by, int timeout = 30)
        {
            var foundElement = StableElementExtensions.GetFirstElement(driver, scope, @by, timeout);
            return new StableWebElement(scope, foundElement, @by, SearchApproachType.First);
        }
    }

    internal static class StableElementExtensions
    {
        internal static string GetElementDescription(this ISearchContext element)
        {
            var stableElement = element as IStableWebElement;
            return stableElement?.GetDescription() ?? string.Empty;
        }


        internal static IWebElement TryFindElement(this ISearchContext context, By by)
        {
            try
            {
                return context.FindElement(by);
            }
            catch
            {
                return null;
            }
        }

        internal static IWebElement FindFirstAccessibleElement(this ISearchContext scope, By locator)
        {
            var candidates = scope.FindElements(locator);
            var foundElement = candidates.FirstAccessibleOrDefault();
            if (foundElement == null)
            {
                throw new CannotFindAccessibleElementByException(locator, scope, candidates);
            }
            return foundElement;
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



        internal static IWebElement GetFirstElement(RemoteWebDriver driver, ISearchContext scope, By @by, int timeout)
        {
            try
            {
                return driver.WaitUntil(timeout, d => scope.FindElement(@by));
            }
            catch (WebDriverTimeoutException ex)
            {
                throw new CannotFindElementByException(@by, scope, ex);
            }
        }

        private static TResult WaitUntil<TResult>(this RemoteWebDriver driver, int timeout, Func<IWebDriver, TResult> waitPredictor)
        {
            var waiter = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
            return waiter.Until(waitPredictor);
        }
    }
}