using MaintainableSelenium.MvcPages.SeleniumUtils;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace MaintainableSelenium.MvcPages.WebPages
{
    public class PageFragment : IPageFragment
    {
        protected readonly RemoteWebDriver Driver;
        protected readonly IWebElement WebElement;

        public PageFragment(RemoteWebDriver driver, IWebElement webElement)
        {
            this.Driver = driver;
            this.WebElement = webElement;
        }
        
        public void ClickOnElementWithText(string text)
        {
            Driver.ClickOnElementWithText(WebElement, text);
        }

        public void HoverOnElementWithText(string text)
        {
            var elementToHover = Driver.GetElementWithText(WebElement, text);
            Driver.HoverOn(elementToHover);
        }

        public WebList GetListWithId(string id)
        {
            return Driver.GetListWithId(id);
        }
    }

    public interface IPageFragment
    {
        void ClickOnElementWithText(string text);
        void HoverOnElementWithText(string text);
        WebList GetListWithId(string id);
    }
}