using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;

namespace MaintainableSelenium.Toolbox.WebPages
{
    public class PageFragment
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
            var by = By.PartialLinkText(linkText);
            ClickOn(@by);
        }
        
        public void ClickOnElementWithLabel(string linkText)
        {
            var by = By.XPath(string.Format("//*[contains(text(), '{0}')]", linkText));
            ClickOn(@by);
        }

        private void ClickOn(By @by)
        {
            var waiter = new WebDriverWait(Driver, TimeSpan.FromSeconds(30));
            var linkElement = waiter.Until(
                (a) =>
                {
                    var link = WebElement.FindElement(@by);
                    if (link != null && link.Displayed && link.Enabled)
                    {
                        return link;
                    }
                    return null;
                });
            linkElement.Click();
        }
    }
}