using OpenQA.Selenium;

namespace Tellurium.MvcPages.WebPages
{
    public class WebList:WebElementCollection<PageFragment>
    {
        public WebList(IBrowserAdapter browserAdapter, IWebElement webElement):base(browserAdapter, webElement)
        {
        }

        protected override IWebElement GetItemsContainer()
        {
            return this.WebElement;
        }

        protected override PageFragment MapToItem(IWebElement webElementItem)
        {
            return new PageFragment(this.Browser, webElementItem);
        }
    }
}