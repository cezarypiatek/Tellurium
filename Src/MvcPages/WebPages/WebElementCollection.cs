using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using OpenQA.Selenium;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Remote;
using Tellurium.MvcPages.SeleniumUtils;
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
                var byIndex = ByIndex.FromIndex(index);
                return GetElementByXPath(byIndex);
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
                return GetElementByXPath(ByIndex.FromLast());
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
                var byIndex =  ByIndex.FromIndex(0);
                return GetElementByXPath(byIndex);
            }
            catch (NoSuchElementException)
            {
                throw new EmptyWebElementCollectionException();
            }
        }

        private TItem GetElementByXPath(By xpath)
        {
            var container = GetItemsContainer();
            var childElement = container.FindStableElement(xpath);
            return MapToItem(childElement);
        }

        public TItem FindItemWithText(string text)
        {
            var selector = ByIndex.FromItemText(text);
            return GetElementByXPath(selector);
        }
    }

    internal class ByIndex : By
    {
        public static ByIndex FromIndex(int index)
        {
            return new ByIndex($"*[{index + 1}]" ,$"At index {index}" );
        }

        public static ByIndex FromLast()
        {
            return new ByIndex("*[last()]", "At last index");
        }

        public static ByIndex FromItemText(string text)
        {
            var xpathLiteral = XPathHelpers.ToXPathLiteral(text.Trim());
            var xpathToFind = $"*[contains(string(), {xpathLiteral})]";
            return new ByIndex(xpathToFind, $"containing partial text: '{text}'");
        }

        private ByIndex(string xpathToFind, string description)
        {
            FindElementMethod = context => ((IFindsByXPath) context).FindElementByXPath(xpathToFind);
            FindElementsMethod = context => ((IFindsByXPath) context).FindElementsByXPath(xpathToFind);
            Description = description;
        }
    }
}