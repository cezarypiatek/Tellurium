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
            Driver.ClickOnElementWithText(WebElement, text, false);
        }

        public void ClickOnElementWithPartialText(string text)
        {
            Driver.ClickOnElementWithText(WebElement, text, true);
        }

        public void HoverOnElementWithText(string text)
        {
            Driver.HoverOnElementWithText(WebElement, text, false);
        }

        public void HoverOnElementWithPartialText(string text)
        {
            Driver.HoverOnElementWithText(WebElement, text, true);
        }

        public WebList GetListWithId(string id)
        {
            return Driver.GetListWithId(id);
        }
    }

    public interface IPageFragment
    {
        void ClickOnElementWithText(string text);
        void ClickOnElementWithPartialText(string text);
        void HoverOnElementWithText(string tex);
        void HoverOnElementWithPartialText(string text);
        WebList GetListWithId(string id);
    }
}