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
        
        public void ClickOnElementWithText(string linkText)
        {
            Driver.ClickOnElementWithText(WebElement, linkText);
        }
    }

    public interface IPageFragment
    {
        void ClickOnElementWithText(string linkText);
    }
}