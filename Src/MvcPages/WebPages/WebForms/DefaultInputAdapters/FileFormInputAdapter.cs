using OpenQA.Selenium;
using Tellurium.MvcPages.SeleniumUtils;
using Tellurium.MvcPages.SeleniumUtils.FileUploading;

namespace Tellurium.MvcPages.WebPages.WebForms.DefaultInputAdapters
{
    public class FileFormInputAdapter:IFormInputAdapter
    {
        public virtual bool CanHandle(IWebElement webElement)
        {
            return webElement.GetInputType() == "file";
        }

        public virtual void SetValue(IWebElement webElement, string value)
        {
            OpenFileSelectDialog(webElement);
            FileUploadingExtensions.UploadFileForCurrentBrowser(value);
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