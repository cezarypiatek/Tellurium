using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using Tellurium.MvcPages.SeleniumUtils;

namespace Tellurium.MvcPages.WebPages
{
    public class WebTable : WebElementCollection<WebTableRow>
    {
        private Dictionary<string, int> columnsMap;

        public WebTable(RemoteWebDriver driver, IWebElement webElement):base(driver, webElement)
        {
            this.columnsMap = GetColumnsMap();
        }

        private IWebElement GetBody()
        {
            return WebElement.TryFindElement(By.TagName("tbody")) ?? WebElement;
        }

        private Dictionary<string, int> GetColumnsMap()
        {
            var result = new Dictionary<string, int>();
            var header = WebElement.TryFindElement(By.TagName("thead"));
            if (header == null)
            {
                return result;
            }
            var columns = header.FindElements(By.TagName("th"));
            
            for (int i = 0; i < columns.Count; i++)
            {
                var columnHeader = columns[i].Text?? string.Empty;
                if (string.IsNullOrWhiteSpace(columnHeader) == false && result.ContainsKey(columnHeader) == false)
                {
                    result.Add(columnHeader, i);
                }
            }
            return result;
        }

        protected override IWebElement GetItemsContainer()
        {
            return GetBody();
        }

        protected override WebTableRow MapToItem(IWebElement webElementItem)
        {
            return new WebTableRow(Driver, webElementItem, columnsMap);
        }
    }
}