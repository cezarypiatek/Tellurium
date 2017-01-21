using System;
using System.Collections.ObjectModel;
using System.Drawing;
using MaintainableSelenium.MvcPages.Utils;
using OpenQA.Selenium;

namespace MaintainableSelenium.MvcPages.SeleniumUtils
{
    public class StableWebElement: IStableWebElement
    {
        private readonly ISearchContext parent;
        private IWebElement element;
        private readonly By elementByIdentifier;

        public StableWebElement(ISearchContext parent, IWebElement element, By elementByIdentifier)
        {
            this.parent = parent;
            this.element = element;
            this.elementByIdentifier = elementByIdentifier;
        }

        public void RegenerateElement()
        {
            var stableParent = parent as IStableWebElement;
            if (stableParent != null && stableParent.IsStale())
            {
                stableParent.RegenerateElement();
            }
            this.element = this.parent.FindElement(elementByIdentifier);
        }

        public bool IsStale()
        {
            try
            {
                //INFO: If element is stale accessing any property should throw exception
                // ReSharper disable once UnusedVariable
                var tagName =this.element.TagName;
                return false;
            }
            catch (StaleElementReferenceException )
            {
                return true;
            }
        }

        private T Execute<T>(Func<T> function)
        {
            T result = default (T);
            Execute(() => { result = function(); });
            return result;
        }

        private void Execute(Action action)
        {
            var success = RetryHelper.Retry(3, () =>
            {
                try
                {
                    action();
                    return true;
                }
                catch (StaleElementReferenceException)
                {
                    RegenerateElement();
                    return false;
                }
            });
            if (success == false)
            {
                throw new WebElementNotFoundException("Element is no longer accessible");
            }
        }

        public IWebElement FindElement(By by)
        {
            return Execute(() => element.FindElement(by));
        }

        public ReadOnlyCollection<IWebElement> FindElements(By by)
        {
            return Execute(() => element.FindElements(by));
        }

        public void Clear()
        {
            Execute(() => element.Clear());
        }

        public void SendKeys(string text)
        {
            Execute(() => element.SendKeys(text));
        }

        public void Submit()
        {
            Execute(() => element.Submit());
        }

        public void Click()
        {
            Execute(() => element.Click());
        }

        public string GetAttribute(string attributeName)
        {
            return Execute(() => element.GetAttribute(attributeName));
        }

        public string GetCssValue(string propertyName)
        {
            return Execute(() => element.GetCssValue(propertyName));
        }

        public string TagName { get { return Execute(() => element.TagName); } }
        public string Text { get { return Execute(() => element.Text); } }
        public bool Enabled { get { return Execute(() => element.Enabled); } }
        public bool Selected { get { return Execute(() => element.Selected); } }
        public Point Location { get { return Execute(() => element.Location); } }
        public Size Size { get { return Execute(() => element.Size); } }
        public bool Displayed { get { return Execute(() => element.Displayed); } }
    }

    public interface IStableWebElement : IWebElement
    {
        void RegenerateElement();
        bool IsStale();
    }
}