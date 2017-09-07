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
        var launchTicket = null;
        window.__selenium_observers__ =  window.__selenium_observers__ || {};
        window.__selenium_observers__[watcherId] = {
                observer: new MutationObserver(function() {
                        clearTimeout(launchTicket);
                        launchTicket = setTimeout(function(){
                            window.__selenium_observers__[watcherId].occured = true;
                            window.__selenium_observers__[watcherId].observer.disconnect();
                        }, 250);
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
                driver.WaitUntil(timeout,
                    d =>
                        (bool)
                        driver.ExecuteScript(
                            "return window.__selenium_observers__ && window.__selenium_observers__[arguments[0]].occured;",
                            watcherId));
            }
            catch (WebDriverTimeoutException ex)
            {
                throw new CannotObserveAnyChanges(element, ex);
            }
            
        }
    }

    public class CannotObserveAnyChanges:ApplicationException
    {
        public IWebElement ObservedElement { get; private set; }

        public CannotObserveAnyChanges(IWebElement observedElement, Exception innerException)
            :base($"No changes has been obverved for element {observedElement.GetElementDescription()}", innerException)
        {
            this.ObservedElement = observedElement;
        }
    }
}