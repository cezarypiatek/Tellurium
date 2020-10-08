using System;
using System.Collections.ObjectModel;
using System.Linq;
using OpenQA.Selenium;
using Tellurium.MvcPages.SeleniumUtils.Exceptions;

namespace Tellurium.MvcPages.SeleniumUtils
{
    internal static class StableElementExtensions
    {
        public static string GetElementDescription(this ISearchContext element)
        {
            var stableElement = element as IStableWebElement;
            return stableElement?.GetDescription() ?? string.Empty;
        }


        public static IWebElement TryFindElement(this ISearchContext context, By by)
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

        public static IWebElement FindFirstAccessibleElement(this ISearchContext scope, By locator)
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
    }
}