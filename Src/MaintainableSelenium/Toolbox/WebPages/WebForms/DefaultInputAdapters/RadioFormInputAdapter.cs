using System.Linq;
using MaintainableSelenium.Toolbox.SeleniumUtils;
using OpenQA.Selenium;

namespace MaintainableSelenium.Toolbox.WebPages.WebForms.DefaultInputAdapters
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
    }
}