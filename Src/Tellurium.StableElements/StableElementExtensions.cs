using System;
using System.Collections.ObjectModel;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using Tellurium.MvcPages.SeleniumUtils.Exceptions;

namespace Tellurium.MvcPages.SeleniumUtils
{
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



        internal static IWebElement GetFirstElement(ISearchContext scope, By @by, int timeout)
        {
            try
            {
                return WaitUntil(timeout, d => scope.FindElement(@by));
            }
            catch (WebDriverTimeoutException ex)
            {
                throw new CannotFindElementByException(@by, scope, ex);
            }
        }

        private static TResult WaitUntil<TResult>(int timeout, Func<object, TResult> waitPredictor)
        {
            var waiter = new StatelessWait(TimeSpan.FromSeconds(timeout));
            return waiter.Until(waitPredictor);
        }
    }
}