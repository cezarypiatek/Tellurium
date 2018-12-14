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


        public WebTableRow(IBrowserAdapter browserAdapter, IWebElement webElement, Dictionary<string, int> columnsMap) : base(browserAdapter, webElement)
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
            return new PageFragment(this.Browser, webElementItem);
        }

        public IPageFragment this[string columnName]
        {
            get
            {
                if (string.IsNullOrWhiteSpace(columnName))
                {
                    throw new ArgumentException("Column name cannot be empty", nameof(columnName));
                }
                if (columnsMap.ContainsKey(columnName) == false)
                {
                    throw new ArgumentException($"There is no column with header {columnName}", nameof(columnName));
                }
                var index = columnsMap[columnName];
                return this[index];
            }
        }
    }
}