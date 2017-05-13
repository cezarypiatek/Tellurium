using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using Tellurium.MvcPages.SeleniumUtils;

namespace Tellurium.MvcPages.WebPages
{
    public class WebTree: WebElementCollection<WebTree>
    {
        private bool IsSelfItemsContainer { get; set; }

        private By ItemsContainerLocator { get; set; }

        public WebTree(RemoteWebDriver driver, IWebElement webElement, bool isSelfItemsContainer=true, By itemsContainerLocator=null) 
            : base(driver, webElement)
        {
            IsSelfItemsContainer = isSelfItemsContainer;
            ItemsContainerLocator = itemsContainerLocator ?? By.TagName(webElement.TagName); ;
        }

        protected override IWebElement GetItemsContainer()
        {
            if (this.IsSelfItemsContainer)
            {
                return this.WebElement;
            }
            return Driver.GetStableAccessibleElementByInScope(ItemsContainerLocator, this.WebElement);
        }

        protected override WebTree MapToItem(IWebElement webElementItem)
        {
            return new WebTree(Driver, webElementItem, false, this.ItemsContainerLocator);
        }
    }
}