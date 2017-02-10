using System;
using System.Collections.ObjectModel;
using System.Drawing;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions.Internal;
using OpenQA.Selenium.Internal;
using Tellurium.MvcPages.Utils;

namespace Tellurium.MvcPages.SeleniumUtils
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

        public IWebElement Unwrap()
        {
            return element;
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

        public Point LocationOnScreenOnceScrolledIntoView
        {
            get { return Execute(() => element.As<ILocatable>().LocationOnScreenOnceScrolledIntoView); }
        }

        public ICoordinates Coordinates
        {
            get { return Execute(() => element.As<ILocatable>().Coordinates); }
        }

        public Screenshot GetScreenshot()
        {
            return Execute(() => element.As<ITakesScreenshot>().GetScreenshot());
        }

        public IWebElement WrappedElement { get { return element; } }
    }

    public interface IStableWebElement : IWebElement, ILocatable, ITakesScreenshot, IWrapsElement
    {
        void RegenerateElement();
        bool IsStale();
    }

    internal static class GenericHelpers
    {
        public static TInterface As<TInterface>(this IWebElement element) where TInterface : class
        {
            var typed = element as TInterface;
            if (typed == null)
            {
                var errorMessage = $"Underlying element does not support this opperation. It should ilement {typeof(TInterface).FullName} interface";
                throw new NotSupportedException(errorMessage);
            }
            return typed;
        }

    }
}