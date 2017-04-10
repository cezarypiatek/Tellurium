using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace Tellurium.MvcPages.WebPages
{
    public class WebList:WebElementCollection<PageFragment>
    {
        public WebList(RemoteWebDriver driver, IWebElement webElement):base(driver, webElement)
        {
        }

        protected override IWebElement GetItemsContainer()
        {
            return this.WebElement;
        }

        protected override PageFragment MapToItem(IWebElement webElementItem)
        {
            return new PageFragment(Driver, webElementItem);
        }
    }
}