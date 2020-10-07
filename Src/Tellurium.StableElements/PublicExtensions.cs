using OpenQA.Selenium;

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

        public static IStableWebElement GetStableElementBy(this ISearchContext scope, By @by, int timeout = 30)
        {
            var foundElement = StableElementExtensions.GetFirstElement(scope, @by, timeout);
            return new StableWebElement(scope, foundElement, @by, SearchApproachType.First);
        }
    }
}