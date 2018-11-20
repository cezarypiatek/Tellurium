using OpenQA.Selenium.Remote;

namespace Tellurium.MvcPages.SeleniumUtils
{
    internal class DefaultPageReloadDetector : IPageReloadDetector
    {
        public void RememberPage(RemoteWebDriver driver)
        {
            MarkPageAsVisited(driver);
        }

        public bool WasReloaded(RemoteWebDriver driver)
        {
            return IsNewlyLoadedPage(driver);
        }


        private bool IsNewlyLoadedPage(RemoteWebDriver driver)
        {
            return driver.IsPageLoaded() && HasVisitedPageMark(driver) == false;
        }

        private bool HasVisitedPageMark(RemoteWebDriver driver)
        {
            return (bool)driver.ExecuteScript("return window.__selenium_visited__ === true;");
        }

        private void MarkPageAsVisited(RemoteWebDriver driver)
        {
            driver.ExecuteScript("window.__selenium_visited__ = true;");
        }
    }
}