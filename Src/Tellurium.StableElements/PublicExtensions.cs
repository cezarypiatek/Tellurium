using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

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
            var @by = By.Id(elementId);
            return GetStableElementByInScope(driver, driver, @by, timeout);
        }

        public static IStableWebElement GetStableElementByInScope(this RemoteWebDriver driver, ISearchContext scope, By by, int timeout = 30)
        {
            var foundElement = StableElementExtensions.GetFirstElement(driver, scope, @by, timeout);
            return new StableWebElement(scope, foundElement, @by, SearchApproachType.First);
        }
    }
}