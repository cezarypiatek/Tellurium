using System.Threading;
using OpenQA.Selenium.Remote;

namespace MaintainableSelenium.MvcPages.BrowserCamera.Lens
{
    public class ZoomableLens:IBrowserCameraLens
    {
        private readonly RemoteWebDriver driver;

        public ZoomableLens(RemoteWebDriver driver)
        {
            this.driver = driver;
        }

        public byte[] TakeScreenshot()
        {
            PreparePageForScreenshot();
            var screenshot = driver.GetScreenshot();
            RestorePageAfterScreenshot();
            return screenshot.AsByteArray;
        }


        void PreparePageForScreenshot()
        {
            driver.ExecuteScript(@"(function(){
var windowHeight = window.innerHeight;
var documentHeight = Math.max(document.body.scrollHeight, document.body.offsetHeight, document.documentElement.clientHeight, document.documentElement.scrollHeight, document.documentElement.offsetHeight);
var zoomCoeff = windowHeight/documentHeight;
if(zoomCoeff < 1) {document.body.style.zoom= zoomCoeff;  }
})();");
            Thread.Sleep(100);
        }


        void RestorePageAfterScreenshot()
        {
            driver.ExecuteScript(@"(function(){document.body.style.zoom= 1;})();");
            Thread.Sleep(100);
        }
    }
}