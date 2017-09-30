using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using Tellurium.MvcPages.SeleniumUtils;

namespace Tellurium.MvcPages.WebPages
{
    public class PageFragmentWatcher
    {
        private readonly RemoteWebDriver driver;
        private readonly IWebElement element;
        private readonly string watcherId;
        
        public PageFragmentWatcher(RemoteWebDriver driver, IWebElement element)
        {
            this.driver = driver;
            this.element = element;
            this.watcherId = Guid.NewGuid().ToString();
        }

        public void StartWatching(bool watchSubtree=true)
        {
            driver.ExecuteScript(@"
(function(target, watcherId, watchSubtree){
        window.__selenium_observers__ =  window.__selenium_observers__ || {};
        window.__selenium_observers__[watcherId] = {
                observer: new MutationObserver(function(mutations) {
                        window.__selenium_observers__[watcherId].occured = true;
                        window.__selenium_observers__[watcherId].observer.disconnect();
                }),
                occured:false
        };
        var config = { attributes: true, childList: true, characterData: true, subtree: watchSubtree };
        window.__selenium_observers__[watcherId].observer.observe(target, config);
})(arguments[0],arguments[1], arguments[2]);", element, watcherId, watchSubtree);
        }

        public void WaitForChange(int timeout = 30)
        {
            try
            {
                driver.WaitUntil(timeout, d =>
                {
                    try
                    {
                        return (bool) driver.ExecuteScript("return window.__selenium_observers__[arguments[0]].occured;", watcherId);
                    }
                    catch (InvalidOperationException)
                    {
                        throw NoChangesException.BecausePageReloaded(element);
                    }
                });

            }
            catch (WebDriverTimeoutException)
            {
                throw NoChangesException.BecauseTimeout(element, timeout);
            }
            
        }
    }

    public class NoChangesException:ApplicationException
    {
        public static NoChangesException BecausePageReloaded(IWebElement observedElement)
        {
            return  new NoChangesException($"Cannot observer any changes for the element {observedElement.GetElementDescription()} because page was reloaded in the meantime");
        }

        public static NoChangesException BecauseTimeout(IWebElement observedElement, int timeoutInSeconds)
        {
            return new NoChangesException($"No changes has been obverved for the element {observedElement.GetElementDescription()} within {timeoutInSeconds}s");
        }

        private NoChangesException(string message) : base(message)
        {

        }
    }
}