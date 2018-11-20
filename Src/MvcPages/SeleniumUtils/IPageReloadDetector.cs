using OpenQA.Selenium.Remote;

namespace Tellurium.MvcPages.SeleniumUtils
{
    public interface IPageReloadDetector
    {
        void RememberPage(RemoteWebDriver driver);
        bool WasReloaded(RemoteWebDriver driver);
    }
}