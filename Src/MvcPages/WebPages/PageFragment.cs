using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Remote;
using Tellurium.MvcPages.SeleniumUtils;

namespace Tellurium.MvcPages.WebPages
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

        public WebList ToWebList()
        {
            return new WebList(Driver, WebElement);
        }

        public WebTable GetTableWithId(string id)
        {
            return Driver.GetTableWithId(id);
        }

        public WebTable ToWebTable()
        {
            return new WebTable(Driver, WebElement);
        }

        public string Text => WebElement.Text;

        public void AffectWith(Action action)
        {
            var watcher = new PageFragmentWatcher(Driver, WebElement);
            watcher.StartWatching();
            action();
            watcher.WaitForChange();
        }

        public IWebElement WrappedElement => WebElement;
    }

    public interface IPageFragment:IWrapsElement
    {
        void ClickOnElementWithText(string text);
        void ClickOnElementWithPartialText(string text);
        void HoverOnElementWithText(string tex);
        void HoverOnElementWithPartialText(string text);
        WebList GetListWithId(string id);
        WebList ToWebList();

        WebTable GetTableWithId(string id);
        WebTable ToWebTable();
        string Text { get; }
        void AffectWith(Action action);
    }
}