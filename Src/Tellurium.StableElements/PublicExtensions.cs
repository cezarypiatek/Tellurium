using System;
using OpenQA.Selenium;
using Tellurium.MvcPages.SeleniumUtils.Exceptions;

namespace Tellurium.MvcPages.SeleniumUtils
{
    public static class PublicExtensions
    {
        /// <summary>
        ///     Search for a given element in the current DOM tree fragment.
        ///     Fail if expected element doesn't exist.
        /// </summary>
        /// <param name="scope">Searching scope</param>
        /// <param name="locator">Expected element locator</param>
        public static IStableWebElement FindStableElement(this ISearchContext scope, By locator)
        {
            var element = scope.FindElement(locator);
            return new StableWebElement(scope, element, locator, SearchApproachType.First);
        }

        /// <summary>
        ///     Search for a given element in the current DOM tree fragment.
        ///     Returns null if expected element doesn't exist.
        /// </summary>
        /// <param name="scope">Searching scope</param>
        /// <param name="locator">Expected element locator</param>
        public static IStableWebElement TryFindStableElement(this ISearchContext scope, By locator)
        {
            var element = scope.TryFindElement(locator);
            if (element == null) return null;
            return new StableWebElement(scope, element, locator, SearchApproachType.First);
        }

        /// <summary>
        ///     Search for a given element in the DOM tree fragment.
        ///     Fail if expected element is not found withing a given time period.
        /// </summary>
        /// <param name="scope">Searching scope</param>
        /// <param name="locator">Expected element locator</param>
        /// <param name="timeout"></param>
        public static IStableWebElement GetStableElement(this ISearchContext scope, By locator, int timeout = 30)
        {
            var foundElement = AwaitElement(scope, locator, timeout);
            return new StableWebElement(scope, foundElement, locator, SearchApproachType.First);
        }


        private static IWebElement AwaitElement(ISearchContext scope, By @by, int timeout)
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