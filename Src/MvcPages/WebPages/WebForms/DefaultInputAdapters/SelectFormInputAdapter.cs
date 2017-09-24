using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Tellurium.MvcPages.WebPages.WebForms.DefaultInputAdapters
{
    public class SelectFormInputAdapter : IFormInputAdapter
    {
        public const char MulipleValueSeparator = '|';

        public bool CanHandle(IWebElement webElement)
        {
            return webElement.TagName.ToLower() == "select";
        }

        public void SetValue(IWebElement webElement, string value)
        {
            var select = new SelectElement(webElement);
            if (select.IsMultiple)
            {
                select.DeselectAll();
                var values = value.Split(MulipleValueSeparator);
                foreach (var singleValue in values)
                {
                    select.SelectByText(singleValue);
                }
            }
            else
            {
                select.SelectByText(value);
            }
            
        }

        public string GetValue(IWebElement webElement)
        {
            SelectElement select = new SelectElement(webElement);
            if (select.IsMultiple)
            {
                var selectedValuesText = @select.AllSelectedOptions.Select(x => x.Text);
                return string.Join(MulipleValueSeparator.ToString(), selectedValuesText);
            }
            return select.SelectedOption.Text;
        }

        public bool SupportSetRetry()
        {
            return true;
        }
    }
}