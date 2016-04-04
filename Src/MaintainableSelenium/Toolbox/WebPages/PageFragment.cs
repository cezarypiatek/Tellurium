using MaintainableSelenium.Toolbox.SeleniumUtils;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace MaintainableSelenium.Toolbox.WebPages
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
        
        public void ClickOnLinkWithLabel(string linkText)
        {
            Driver.ClickOnLinkWithLabel(WebElement, linkText);
        }
        
        public void ClickOnElementWithLabel(string linkText)
        {
            Driver.ClickOnElementWithLabel(WebElement, linkText);
        }
    }

    public interface IPageFragment
    {
        void ClickOnLinkWithLabel(string linkText);
        void ClickOnElementWithLabel(string linkText);
    }
}