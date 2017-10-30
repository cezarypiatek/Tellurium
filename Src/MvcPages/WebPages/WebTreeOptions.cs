using OpenQA.Selenium;
using Tellurium.MvcPages.SeleniumUtils;

namespace Tellurium.MvcPages.WebPages
{
    public class WebTreeOptions
    {
        public bool IsSelfItemsContainer { get; set; }
        public By ItemsContainerLocator { get; set; }
        public string ExpanderText { get; set; }
        public string CollapserText { get; set; }
        public By ExpanderLocator { get; set; }
        public By CollapserLocator { get; set; }

        public WebTreeOptions()
        {
            this.IsSelfItemsContainer = true;
        }

        internal By GetEffectiveExpanderLocator()
        {
            return GetEffectiveLocator(ExpanderLocator, ExpanderText);
        }

        internal By GetEffectiveCollapseLocator()
        {
            return GetEffectiveLocator(CollapserLocator, CollapserText);
        }

        private static By GetEffectiveLocator(By locator, string expanderText)
        {
            if (locator != null)
            {
                return locator;
            }

            if (string.IsNullOrWhiteSpace(expanderText) == false)
            {
                return ByText.From(expanderText);
            }
            return null;
        }
    }
}