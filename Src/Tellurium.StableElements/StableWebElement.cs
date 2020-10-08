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
                this.element = searchApproach switch
                {
                    SearchApproachType.First => this.parent.FindElement(locator),
                    SearchApproachType.FirstAccessible => this.parent.FindFirstAccessibleElement(locator),
                    _ => throw new NotSupportedException($"Value '{searchApproach}' for '{nameof(searchApproach)}' is not supported.")
                };
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
                throw new CannotFindElementByException(this.locator, this.parent);
            }
        }

        public IWebElement FindElement(By by)
        {
            var foundElement = Execute(() => element.FindElement(@by));
            return new StableWebElement(this,  foundElement, by, SearchApproachType.First);
        }

        public ReadOnlyCollection<IWebElement> FindElements(By by) => Execute(() => element.FindElements(@by));
        public void Clear() => Execute(() => element.Clear());
        public void SendKeys(string text) => Execute(() => element.SendKeys(text));
        public void Submit() => Execute(() => element.Submit());
        public void Click() => Execute(() => element.Click());
        public string GetAttribute(string attributeName) => Execute(() => element.GetAttribute(attributeName));
        public string GetProperty(string propertyName) => Execute(() => element.GetProperty(propertyName));
        public string GetCssValue(string propertyName) => Execute(() => element.GetCssValue(propertyName));
        public string TagName => Execute(() => element.TagName);
        public string Text => Execute(() => element.Text);
        public bool Enabled => Execute(() => element.Enabled);
        public bool Selected => Execute(() => element.Selected);
        public Point Location => Execute(() => element.Location);
        public Size Size => Execute(() => element.Size);
        public bool Displayed => Execute(() => element.Displayed);
        public Point LocationOnScreenOnceScrolledIntoView => Execute(() => element.As<ILocatable>().LocationOnScreenOnceScrolledIntoView);
        public ICoordinates Coordinates => Execute(() => element.As<ILocatable>().Coordinates);
        public Screenshot GetScreenshot() => Execute(() => element.As<ITakesScreenshot>().GetScreenshot());
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