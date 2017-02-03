using System.Linq;
using OpenQA.Selenium;
using Tellurium.MvcPages.SeleniumUtils;

namespace Tellurium.MvcPages.WebPages.WebForms.DefaultInputAdapters
{
    public class RadioFormInputAdapter : IFormInputAdapter
    {
        public bool CanHandle(IWebElement webElement)
        {
            return webElement.GetInputType() == "radio";
        }

        public void SetValue(IWebElement webElement, string value)
        {
            webElement.GetParent().GetParent().FindElements(By.TagName("label")).Single(x =>x.Text == value).Click();
        }

        public string GetValue(IWebElement webElement)
        {
            return webElement.GetParent().GetParent().FindElements(By.TagName("label")).Single(x => x.Selected).Text;
        }

        public bool SupportSetRetry()
        {
            return true;
        }
    }
}