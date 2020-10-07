using System;
using System.Collections.ObjectModel;
using System.Drawing;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions.Internal;
using OpenQA.Selenium.Internal;
using Tellurium.MvcPages.SeleniumUtils.Exceptions;
using Tellurium.StableElements;

namespace Tellurium.MvcPages.SeleniumUtils
{
    public class StableWebElement: IStableWebElement
    {
        private readonly ISearchContext parent;
        private IWebElement element;
        private readonly By locator;
        private readonly SearchApproachType searchApproach;

        public StableWebElement(ISearchContext parent, IWebElement element, By locator, SearchApproachType searchApproach)
        {
            this.parent = parent;
            this.element = element;
            this.locator = locator;
            this.searchApproach = searchApproach;
        }

        public void RegenerateElement()
        {
            if (parent is IStableWebElement stableParent && stableParent.IsStale())
            {
                stableParent.RegenerateElement();
            }
            try
            {
                switch (searchApproach)
                {
                    case SearchApproachType.First:
                        this.element = this.parent.FindElement(locator);
                        break;
                    case SearchApproachType.FirstAccessible:
                        this.element = this.parent.FindFirstAccessibleElement(locator);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(searchApproach));
                }
            }
            catch(Exception ex)
            {
                throw new CannotFindElementByException(locator, parent, ex);  
            }
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

        public string GetDescription()
        {
            var parentDescription = parent is IStableWebElement stableParent ? stableParent.GetDescription() : null;
            var thisDescription = $"'{this.locator}'";
            return parentDescription == null ? thisDescription : $"{thisDescription} inside {parentDescription}";
        }

        public IWebElement Unwrap()
        {
            if (IsStale())
            {
                RegenerateElement();
            }
            return element;
        }

        public T ExecuteSafe<T>(Func<IWebElement, T> action)
        {
            T result = default(T);
            Execute(() => result =  action(element));
            return result;
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

        public string GetProperty(string propertyName)
        {
            return Execute(() => element.GetProperty(propertyName));
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

        public IWebElement WrappedElement => Unwrap();

        public IWebDriver WrappedDriver
        {
            get
            {
                if (element is IWrapsDriver driverWrapper)
                {
                    return driverWrapper.WrappedDriver;
                }
                throw new NotSupportedException($"Element {this.GetDescription()} does not have information about driver");
            }
        }
    }

    public enum SearchApproachType
    {
        First,
        FirstAccessible
    }
}