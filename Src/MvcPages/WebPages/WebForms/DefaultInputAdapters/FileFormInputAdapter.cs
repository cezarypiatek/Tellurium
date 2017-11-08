using OpenQA.Selenium;
using Tellurium.MvcPages.SeleniumUtils;
using Tellurium.MvcPages.SeleniumUtils.ChromeRemoteInterface;
using Tellurium.MvcPages.SeleniumUtils.FileUploading;

namespace Tellurium.MvcPages.WebPages.WebForms.DefaultInputAdapters
{
    public class FileFormInputAdapter:IFormInputAdapter
    {
        private ChromeRemoteInterface chromeremoteInterface;

        public virtual bool CanHandle(IWebElement webElement)
        {
            return webElement.GetInputType() == "file";
        }

        public virtual void SetValue(IWebElement webElement, string value)
        {
            if (TrySetWithRemoteInterface(webElement, value))
            {
                return;
            }
            OpenFileSelectDialog(webElement);
            FileUploadingExtensions.UploadFileForCurrentBrowser(value);
        }

        private bool TrySetWithRemoteInterface(IWebElement webElement, string value)
        {
            var remoteWebDriver = webElement.GetWebDriver();
            if (ChromeRemoteInterface.IsSupported(remoteWebDriver) == false)
            {
                return false;
            }

            if (chromeremoteInterface == null)
            {
                chromeremoteInterface = new ChromeRemoteInterface(remoteWebDriver);
            }
            chromeremoteInterface.SetFileInputFiles(webElement, new []{value});
            return true;
        }

        protected virtual void OpenFileSelectDialog(IWebElement webElement)
        {
            webElement.Click();
        }

        public virtual string GetValue(IWebElement webElement)
        {
            return webElement.GetAttribute("value");
        }

        public virtual bool SupportSetRetry()
        {
            //INFO: Chrome always return c:\fakepath\file_name.ext instead of real path so the SetRetry will be not working correctly
            return false;
        }
    }
}