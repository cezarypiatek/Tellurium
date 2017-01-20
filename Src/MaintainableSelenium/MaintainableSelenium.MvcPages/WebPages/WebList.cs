using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MaintainableSelenium.MvcPages.Utils;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace MaintainableSelenium.MvcPages.WebPages
{
    public class WebList : IReadOnlyList<IPageFragment>
    {
        private readonly RemoteWebDriver driver;
        private readonly IWebElement webElement;

        public WebList(RemoteWebDriver driver, IWebElement webElement)
        {
            this.driver = driver;
            this.webElement = webElement;
        }

        public IPageFragment this[int index]
        {
            get
            {
                try
                {
                    var selectorToFind = string.Format("*[{0}]", index + 1);
                    return GetPageFragmentByXPath(selectorToFind);
                }
                catch (NoSuchElementException ex)
                {
                    var exceptionMessage = string.Format("Unable to locate child element on index {0}", index);
                    throw new IndexOutOfRangeException(exceptionMessage, ex);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<IPageFragment> GetEnumerator()
        {
            foreach (var childElement in GetChildElements())
                yield return new PageFragment(driver, childElement);
        }

        public int Count
        {
            get
            {
                var childElements = GetChildElements();
                return childElements.Count;
            }
        }

        private ReadOnlyCollection<IWebElement> GetChildElements()
        {
            return webElement.FindElements(By.XPath("*"));
        }

        public IPageFragment Last()
        {
            try
            {
                return GetPageFragmentByXPath("*[last()]");
            }
            catch (NoSuchElementException)
            {
                throw new EmptyWebListException();
            }
        }

        public IPageFragment First()
        {
            try
            {
                return GetPageFragmentByXPath("*[1]");
            }
            catch (NoSuchElementException)
            {
                throw new EmptyWebListException();
            }
        }

        private IPageFragment GetPageFragmentByXPath(string selector)
        {
            var childElement = webElement.FindElement(By.XPath(selector));
            return new PageFragment(driver, childElement);
        }

        public IPageFragment FindItemWithText(string text)
        {
            var xpathLiteral = XPathHelpers.ToXPathLiteral(text);
            var selector = string.Format("*[contains(string(), {0})]",xpathLiteral);
            return GetPageFragmentByXPath(selector);
        }
    }

    public class EmptyWebListException : ApplicationException
    {
        public EmptyWebListException() : base("List is empty")
        {
        }
    }
}