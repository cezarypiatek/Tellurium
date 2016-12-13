using MaintainableSelenium.MvcPages.SeleniumUtils;
using OpenQA.Selenium.Remote;

namespace MaintainableSelenium.MvcPages.WebPages
{
    public class PageFragmentWatcher
    {
        private readonly RemoteWebDriver driver;
        private readonly string containerId;

        public PageFragmentWatcher(RemoteWebDriver driver, string containerId)
        {
            this.driver = driver;
            this.containerId = containerId;
        }

        public void StartWatching()
        {
            driver.ExecuteScript(@"var $$expectedId = arguments[0];
__selenium_observers__ =  window.__selenium_observers__ || {};
(function(){		
		var target = document.getElementById($$expectedId);
		__selenium_observers__[$$expectedId] = {
				observer: new MutationObserver(function(mutations) {
					__selenium_observers__[$$expectedId].occured = true;
					__selenium_observers__[$$expectedId].observer.disconnect();
				}),
				occured:false
		};
		var config = { attributes: true, childList: true, characterData: true, subtree: true };

		__selenium_observers__[$$expectedId].observer.observe(target, config);
})();", containerId);
        }

        public void WaitForChange(int timeout = 30)
        {
            driver.WaitUntil(timeout, d => (bool)driver.ExecuteScript("return window.__selenium_observers__ && window.__selenium_observers__[arguments[0]].occured;", containerId));
        }
    }
}