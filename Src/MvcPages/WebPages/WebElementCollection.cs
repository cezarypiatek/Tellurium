using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using Tellurium.MvcPages.Utils;

namespace Tellurium.MvcPages.WebPages
{
    public abstract class WebElementCollection<TItem> : PageFragment,  IReadOnlyList<TItem> where TItem: IPageFragment
    {
        protected WebElementCollection(RemoteWebDriver driver, IWebElement webElement)
            :base(driver, webElement)
        {
        }

        protected abstract IWebElement GetItemsContainer();
        protected abstract TItem MapToItem(IWebElement webElementItem);

        public TItem this[int index]
        {
            get
            {
                try
                {
                    var selectorToFind = $"*[{index + 1}]";
                    return GetPageFragmentByXPath(selectorToFind);
                }
                catch (NoSuchElementException ex)
                {
                    var exceptionMessage = $"Unable to locate child element on index {index}";
                    throw new IndexOutOfRangeException(exceptionMessage, ex);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<TItem> GetEnumerator()
        {
            foreach (var childElement in GetChildElements())
                yield return MapToItem(childElement);
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
            return GetItemsContainer().FindElements(By.XPath("*"));
        }

        public TItem Last()
        {
            try
            {
                return GetPageFragmentByXPath("*[last()]");
            }
            catch (NoSuchElementException)
            {
                throw new EmptyWebElementCollectionException();
            }
        }

        public TItem First()
        {
            try
            {
                return GetPageFragmentByXPath("*[1]");
            }
            catch (NoSuchElementException)
            {
                throw new EmptyWebElementCollectionException();
            }
        }

        private TItem GetPageFragmentByXPath(string selector)
        {
            var childElement = GetItemsContainer().FindElement(By.XPath(selector));
            return MapToItem(childElement);
        }

        public TItem FindItemWithText(string text)
        {
            var xpathLiteral = XPathHelpers.ToXPathLiteral(text);
            var selector = $"*[contains(string(), {xpathLiteral})]";
            return GetPageFragmentByXPath(selector);
        }
    }
}