using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Threading;
using OpenQA.Selenium.Internal;
using Tellurium.MvcPages.SeleniumUtils.Exceptions;
using Tellurium.MvcPages.Utils;
using Tellurium.MvcPages.WebPages;

namespace Tellurium.MvcPages.SeleniumUtils
{
    public static class SeleniumExtensions
    {
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

        public static void MoveMouseOffTheScreen(this RemoteWebDriver driver)
        {
            var body = driver.FindElementByTagName("body");
            var scrollY = driver.GetScrollY();
            new Actions(driver).MoveToElement(body, 0, scrollY + 1).Perform();
        }

        /// <summary>
        /// Tyoe text into field
        /// </summary>
        /// <param name="input">Field</param>
        /// <param name="text">Text to tyoe</param>
        /// <param name="speed">Speed of typing (chars per minute). 0 means default selenium speed</param>
        public static void Type(this IWebElement input, string text, int speed = 0)
        {
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

        /// <summary>
        /// Clear textual input without generating change event
        /// </summary>
        /// <param name="input">Input element</param>
        public static void ClearTextualInput(this IWebElement input)
        {
            var oldValue = input.GetAttribute("value");
            if (string.IsNullOrEmpty(oldValue) == false)
            {
                input.SendKeys(Keys.End);
                input.SendKeys("".PadLeft(oldValue.Length, Keys.Backspace[0]));
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
        
        internal static PageFragmentWatcher WatchForContentChanges(this RemoteWebDriver driver, string containerId, bool watchSubtree=true)
        {
            var element = driver.GetStableElementById(containerId);
            var watcher = new PageFragmentWatcher(driver, element);
            watcher.StartWatching(watchSubtree);
            return watcher;
        }


        private static bool IsElementInteractable(RemoteWebDriver driver, IWebElement element)
        {
            //INFO: There is a few case when the mouse click can omit actual element
            // 1) Element has rounded corners
            // 2) In inline-element space between lines is not clickable
            // 3) Given element has child elements
            return (bool) driver.ExecuteScript(@"
                    return (function(element)
                    {
                        function belongsToElement(subElement)
                        {
                            return element == subElement || element.contains(subElement);
                        }
                        var rec = element.getBoundingClientRect();
                        var elementAtPosition1 = document.elementFromPoint(rec.left, rec.top);
                        var elementAtPosition2 = document.elementFromPoint(rec.left+rec.width/2, rec.top+rec.height/2);
                        var elementAtPosition3 = document.elementFromPoint(rec.left+rec.width/3, rec.top+rec.height/3);
                        return belongsToElement(elementAtPosition1) || belongsToElement(elementAtPosition2) || belongsToElement(elementAtPosition3);
                    })(arguments[0]);
                ", element);
        }

        public static int GetPageHeight(this RemoteWebDriver driver)
        {
            var scriptResult = driver.ExecuteScript("return Math.max(document.body.scrollHeight, document.body.offsetHeight, document.documentElement.clientHeight, document.documentElement.scrollHeight, document.documentElement.offsetHeight);");
            return (int)(long)scriptResult;
        }


        public static Size GetPageDimensions(this RemoteWebDriver driver)
        {
            var obj = (Dictionary<string, object>)driver.ExecuteScript(
                @"return {
                            width: Math.max(document.body.scrollWidth, document.body.offsetWidth, document.documentElement.clientWidth, document.documentElement.scrollWidth, document.documentElement.offsetWidth), 
                            height: Math.max(document.body.scrollHeight, document.body.offsetHeight, document.documentElement.clientHeight, document.documentElement.scrollHeight, document.documentElement.offsetHeight)
                        };");
            return new Size((int)(long)obj["width"], (int)(long)obj["height"]);
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

        internal static readonly By ParentSelector = By.XPath("..");

        /// <summary>
        /// Return parent of given web element
        /// </summary>
        /// <param name="node">Child element</param>
        public static IWebElement GetParent(this IWebElement node)
        {
            return node.FindElement(ParentSelector);
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
            var expectedElement = driver.GetStableElementWithText(scope, linkText, isPartialText);
            ClickOn(driver, expectedElement);
        }

        public static  void HoverOnElementWithText(this RemoteWebDriver driver, ISearchContext scope, string linkText, bool isPartialText)
        {
            var expectedElement = driver.GetStableElementWithText(scope, linkText, isPartialText);
            HoverOn(driver, expectedElement);
        }

        internal static void ClickOn(this RemoteWebDriver driver, IWebElement expectedElement)
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
                try
                {
                    driver.WaitUntil(SeleniumFinderExtensions.SearchElementDefaultTimeout, (d) => IsStableElementInteractable(driver, expectedElement));
                }
                catch (WebDriverTimeoutException)
                {
                    throw new ElementIsNotClickableException(expectedElement);
                }
                
                expectedElement.Click();
                if (originalScrollPosition != null)
                {
                    driver.ScrollToY(originalScrollPosition.Value);
                }
            }
        }

        private static bool IsStableElementInteractable(RemoteWebDriver driver, IWebElement expectedElement)
        {
            if (expectedElement is StableWebElement stableElement)
            {
                return stableElement.ExecuteSafe((el)=> IsElementInteractable(driver, el));
            }
            return IsElementInteractable(driver, expectedElement);
        }

        public static void HoverOn(this RemoteWebDriver driver, IWebElement elementToHover)
        {
            var action  = new Actions(driver);
            action.MoveToElement(elementToHover).Perform();
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
            return (bool)driver.ExecuteScript(@"return document && document.readyState == 'complete';");
        }

        internal static void WaitUntilPageLoad(this RemoteWebDriver driver)
        {
            driver.WaitUntil(PageLoadTimeout, _ => driver.IsPageLoaded());
        }

        internal static IWebElement GetActiveElement(this RemoteWebDriver driver)
        {
            return (IWebElement) driver.ExecuteScript("return document.activeElement;");
        }


        internal static string GetElementDescription(this ISearchContext element)
        {
            var stableElement = element as IStableWebElement;
            return stableElement?.GetDescription() ?? string.Empty;
        }

        internal static RemoteWebDriver GetWebDriver(this IWebElement webElement)
        {
            return  ((IWrapsDriver)webElement).WrappedDriver as RemoteWebDriver;
        }


        internal static string GetBrowserName(this ICapabilities capabilities)
        {
            return GetSaveCapability(capabilities, "browserName");
        }

        private static string GetSaveCapability(ICapabilities capabilities, string browsername)
        {
            if (capabilities.HasCapability(browsername))
            {
                return capabilities.GetCapability(browsername)?.ToString() ?? string.Empty;
            }

            return string.Empty;
        }

        internal static string GetVersion(this ICapabilities capabilities)
        {
            return GetSaveCapability(capabilities, "version");
        }
    }
}