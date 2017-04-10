using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace Tellurium.MvcPages.WebPages
{
    public class WebTableRow : WebElementCollection<PageFragment>
    {
        private readonly IWebElement webElement;
        private Dictionary<string, int> columnsMap;


        public WebTableRow(RemoteWebDriver driver, IWebElement webElement, Dictionary<string, int> columnsMap) : base(driver, webElement)
        {
            this.webElement = webElement;
            this.columnsMap = columnsMap;
        }

        protected override IWebElement GetItemsContainer()
        {
            return webElement;
        }

        protected override PageFragment MapToItem(IWebElement webElementItem)
        {
            return new PageFragment(Driver, webElementItem);
        }

        public IPageFragment this[string columnName]
        {
            get
            {
                if (string.IsNullOrWhiteSpace(columnName))
                {
                    throw new ArgumentException("Column name cannot be empty", nameof(columnName));
                }
                var index = columnsMap[columnName];
                return this[index];
            }
        }
    }
}