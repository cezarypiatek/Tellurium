using System;
using System.Diagnostics.Contracts;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using Tellurium.MvcPages.Utils;
using Tellurium.MvcPages.WebPages;

namespace Tellurium.MvcPages.SeleniumUtils
{
    public static class SeleniumExtensions
    {
        private const int SearchElementDefaultTimeout = 30;
        internal const int PageLoadTimeout = 120;

        /// <summary>
        /// Get rid of focus from currently focused element
        /// </summary>
        /// <param name="driver">Selenium webdriver</param>
        public static void Blur(this RemoteWebDriver driver)
        {
            driver.SwitchTo().DefaultContent();
            RetryHelper.Retry(3, () =>
            {
                ExecuteBlur(driver);
                Thread.Sleep(500);
                return IsThereElementWithFocus(driver) == false;
            });
        }

        private static void ExecuteBlur(RemoteWebDriver driver)
        {
            driver.ExecuteScript(
                @"(function(){
                        var f= document.querySelector(':focus'); 
                        if(f!=undefined){
                            f.blur();
                        }
                       })();");
        }

        /// <summary>
        /// Tyoe text into field
        /// </summary>
        /// <param name="input">Field</param>
        /// <param name="text">Text to tyoe</param>
        /// <param name="speed">Speed of typing (chars per minute). 0 means default selenium speed</param>
        public static void Type(this IWebElement input, string text, int speed = 0)
        {
            input.Focus();

            if (speed == 0)
            {
                input.SendKeys(text);
            }
            else
            {
                var delay = (1000*60)/speed;
                foreach (var charToType in text)
                {
                    input.SendKeys(charToType.ToString());
                    Thread.Sleep(delay);
                }
            }
        }

        public static void Focus(this IWebElement input)
        {
            input.SendKeys("");
        }

        public static int GetVerticalScrollWidth(this RemoteWebDriver driver)
        {
            //INFO: It's hard to get scrollbar width using JS. 17 its default size of scrollbar on Ms Windows platform
            return 17;
        }

        public static int GetWindowHeight(this RemoteWebDriver driver)
        {
            return (int)(long)driver.ExecuteScript("return window.innerHeight");
        } 
        
        internal static PageFragmentWatcher WatchForContentChanges(this RemoteWebDriver driver, string containerId)
        {
            var watcher = new PageFragmentWatcher(driver, containerId);
            watcher.StartWatching();
            return watcher;
        }


        internal static bool IsElementClickable(this RemoteWebDriver driver, IWebElement element)
        {
            //INFO: There is a few case when the mouse click can ommit actual element
            // 1) Element has rounded corners
            // 2) In inline-element space between lines is not clickable
            // 3) Given element has child elements
            return (bool)driver.ExecuteScript(@"
                    window.__selenium__isElementClickable = window.__selenium__isElementClickable || function(element)
                    {
                        var rec = element.getBoundingClientRect();
                        var elementAtPosition1 = document.elementFromPoint(rec.left, rec.top);
                        var elementAtPosition2 = document.elementFromPoint(rec.left+rec.width/2, rec.top+rec.height/2);
                        var elementAtPosition3 = document.elementFromPoint(rec.left+rec.width/3, rec.top+rec.height/3);
                        return element == elementAtPosition1 || element.contains(elementAtPosition1) || element == elementAtPosition2 || element.contains(elementAtPosition2) || element == elementAtPosition3 || element.contains(elementAtPosition3);
                    };
                    return window.__selenium__isElementClickable(arguments[0]);
            ", element);
        }

        public static int GetPageHeight(this RemoteWebDriver driver)
        {
            var scriptResult = driver.ExecuteScript("return Math.max(document.body.scrollHeight, document.body.offsetHeight, document.documentElement.clientHeight, document.documentElement.scrollHeight, document.documentElement.offsetHeight);");
            return (int)(long)scriptResult;
        }

        public static void ScrollToY(this RemoteWebDriver driver, int y)
        {
            driver.ExecuteScript($"window.scrollTo(0,{y})");
            Thread.Sleep(100);
        }

        public static int GetScrollY(this RemoteWebDriver driver)
        {
            return (int)(long)driver.ExecuteScript(@"
                if(typeof window.scrollY != 'undefined'){
                    return window.scrollY;
                }else if(typeof document.documentElement.scrollTop != 'undefined'){
                    return document.documentElement.scrollTop;
                }
                return 0;
");
        }

        /// <summary>
        /// Check if any element has currently focus
        /// </summary>
        /// <param name="driver">Selenium webdriver</param>
        [Pure]
        private static bool IsThereElementWithFocus(RemoteWebDriver driver)
        {
            return (bool) driver.ExecuteScript("return document.querySelector(':focus')!=undefined;");
        }

        /// <summary>
        /// Search for element with given id
        /// </summary>
        /// <param name="driver">Selenium driver</param>
        /// <param name="elementId">Id of expected element</param>
        /// <param name="timeout">Timout for element serch</param>
        public static IWebElement GetElementById(this RemoteWebDriver driver, string elementId, int timeout = SearchElementDefaultTimeout)
        {
            try
            {
                return driver.GetElementBy(By.Id(elementId), timeout);
            }
            catch (WebDriverTimeoutException ex)
            {
                if (ex.InnerException is NoSuchElementException)
                {
                    var message = $"Cannot find element with id='{elementId}'";
                    throw new WebElementNotFoundException(message, ex);
                }
                throw;
            }
        }

        /// <summary>
        /// Search for element using <see cref="By"/> criterion
        /// </summary>
        /// <param name="driver">Selenium driver</param>
        /// <param name="by"><see cref="By"/> criterion for given element</param>
        /// <param name="timeout">Timout for element serch</param>
        private static IWebElement GetElementBy(this RemoteWebDriver driver, By by, int timeout = SearchElementDefaultTimeout)
        {
            return GetElementByInScope(driver, by, driver, timeout);
        }

        private static IWebElement GetElementByInScope(RemoteWebDriver driver, By @by, ISearchContext scope, int timeout = SearchElementDefaultTimeout)
        {
            var foundElement = driver.WaitUntil(timeout, (a) =>
            {
                try
                {
                    var element = scope.FindElement(@by);
                    if (element != null && element.Displayed && element.Enabled)
                    {
                        return element;
                    }
                    return null;
                }
                catch (StaleElementReferenceException)
                {
                    return null;
                }
            });
            return new StableWebElement(driver, foundElement, by);
        }

        internal static TResult WaitUntil<TResult>(this RemoteWebDriver driver, int timeout,
            Func<IWebDriver, TResult> waitPredictor)
        {
            var waiter = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
            return waiter.Until(waitPredictor);
        }

        internal static IStableWebElement FindStableWebElement(this RemoteWebDriver driver, IWebElement parent, By by, int timeout = 30)
        {
            var waiter = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
            var foundElement = waiter.Until(d => parent.FindElement(@by));
            return new StableWebElement(parent,foundElement,by);
        }

        /// <summary>
        /// Return parent of given web element
        /// </summary>
        /// <param name="node">Child element</param>
        public static IWebElement GetParent(this IWebElement node)
        {
            return node.FindElement(By.XPath(".."));
        }

        /// <summary>
        /// Return type of input represented by the given web element
        /// </summary>
        /// <param name="inputElement">Web element</param>
        public static string GetInputType(this IWebElement inputElement)
        {
            var inputType = inputElement.GetAttribute("type") ?? string.Empty;
            return inputType.ToLower();
        }

        /// <summary>
        /// Click on any element with given text
        /// </summary>
        /// <param name="driver">Selenium driver</param>
        /// <param name="scope">Scope element to narrow link search</param>
        /// <param name="linkText">Element tekst</param>
        public static  void ClickOnElementWithText(this RemoteWebDriver driver, ISearchContext scope, string linkText, bool isPartialText)
        {
            var expectedElement = GetElementWithText(driver, scope, linkText, isPartialText);
            ClickOn(driver, expectedElement);
        }

        public static  void HoverOnElementWithText(this RemoteWebDriver driver, ISearchContext scope, string linkText, bool isPartialText)
        {
            var expectedElement = GetElementWithText(driver, scope, linkText, isPartialText);
            HoverOn(driver, expectedElement);
        }

        public static void ClickOn(this RemoteWebDriver driver, IWebElement expectedElement)
        {
            try
            {
                expectedElement.Click();
            }
            catch (Exception ex)
            {
                //INFO: Different driver throws different exception
                if (ex is InvalidOperationException == false  && ex is WebDriverException == false)
                {
                    throw;
                }

                int? originalScrollPosition = null;
                if (expectedElement.Location.Y > driver.GetWindowHeight())
                {
                    originalScrollPosition = driver.GetScrollY();
                    driver.ScrollToY(expectedElement.Location.Y + expectedElement.Size.Height);
                    Thread.Sleep(500);
                }
                driver.WaitUntil(SearchElementDefaultTimeout, (d) => driver.IsElementClickable(expectedElement));
                expectedElement.Click();
                if (originalScrollPosition != null)
                {
                    driver.ScrollToY(originalScrollPosition.Value);
                }
            }
        }

        public static void HoverOn(this RemoteWebDriver driver, IWebElement elementToHover)
        {
            var action  = new Actions(driver);
            action.MoveToElement(elementToHover).Perform();
        }

        public static IWebElement GetElementWithText(this RemoteWebDriver driver, ISearchContext scope, string linkText, bool isPartialText)
        {
            try
            {
                var xpathLiteral = XPathHelpers.ToXPathLiteral(linkText.Trim());
                var by = isPartialText
                    ? By.XPath(string.Format(".//*[contains(text(), {0}) or (@type='submit' and @value={0}) or (@title={0})]", xpathLiteral))
                    : By.XPath(string.Format(".//*[(normalize-space(text()) = {0}) or (@type='submit' and @value={0}) or (@title={0})]", xpathLiteral));
                return GetElementByInScope(driver, by, scope);
            }
            catch (WebDriverTimeoutException ex)
            {
                if (ex.InnerException is NoSuchElementException)
                {
                    var message = $"Cannot find element with text='{linkText}'";
                    throw new WebElementNotFoundException(message, ex);
                }
                throw;
            }
        }

        internal static void AppendHtml(this RemoteWebDriver driver, IWebElement element, string html)
        {
            driver.ExecuteScript(@"
                (function(parent,text){
                    var wrapper = document.createElement(""div"");
                    wrapper.innerHTML = text;
                    for(var i=0; i< wrapper.childNodes.length; i++)
                    {
                        parent.appendChild(wrapper.childNodes[i]);
                    }
                })(arguments[0],arguments[1]);", element, html);
        }

        internal static bool IsPageLoaded(this RemoteWebDriver driver)
        {
            return (bool)driver.ExecuteScript(@"return document.readyState == 'complete';");
        }

        internal static void WaitUntilPageLoad(this RemoteWebDriver driver)
        {
            driver.WaitUntil(PageLoadTimeout, _ => driver.IsPageLoaded());
        }

        internal static WebList GetListWithId(this RemoteWebDriver driver, string id)
        {
            var listElement = driver.GetElementById(id);
            return new WebList(driver, listElement);
        }

        internal static IWebElement GetActiveElement(this RemoteWebDriver driver)
        {
            return (IWebElement) driver.ExecuteScript("return document.activeElement;");
        }
    }
}