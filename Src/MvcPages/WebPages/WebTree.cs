using System;
using OpenQA.Selenium;
using Tellurium.MvcPages.SeleniumUtils;

namespace Tellurium.MvcPages.WebPages
{
    public class WebTree: WebElementCollection<WebTree>
    {
        private readonly By expanderLocator;
        private readonly By collapserLocator;
        private readonly bool isSelfItemsContainer;
        private readonly By itemsContainerLocator;


        public WebTree(IBrowserAdapter browserAdapter, IWebElement webElement, WebTreeOptions customOptions=null) 
            : base(browserAdapter, webElement)
        {
            var options = customOptions ?? new WebTreeOptions();
            this.expanderLocator = options.GetEffectiveExpanderLocator();
            this.collapserLocator = options.GetEffectiveCollapseLocator();
            this.isSelfItemsContainer = options.IsSelfItemsContainer;
            this.itemsContainerLocator = options.ItemsContainerLocator ?? By.TagName(webElement.TagName); ;
        }

        protected override IWebElement GetItemsContainer()
        {
            if (this.isSelfItemsContainer)
            {
                return this.WebElement;
            }
            return this.WebElement.TryFindStableElement(itemsContainerLocator);
        }

        protected override WebTree MapToItem(IWebElement webElementItem)
        {
            return new WebTree(this.Browser, webElementItem, new WebTreeOptions
            {
                IsSelfItemsContainer = false, 
                ItemsContainerLocator = this.itemsContainerLocator, 
                ExpanderLocator = this.expanderLocator, 
                CollapserLocator = this.collapserLocator
            });
        }

        public void Expand()
        {
            var expander = this.WebElement.TryFindElement(this.expanderLocator);
            PerformToggle(expander);
        }

        public void Collapse()
        {
            var collapser = this.WebElement.TryFindElement(this.collapserLocator);
            PerformToggle(collapser);
        }

        private void PerformToggle(IWebElement expander)
        {
            if (expander != null && expander.Displayed)
            {
                this.AffectWith(() => expander.Click());
            }
        }

        public void ExpandAll()
        {
            WalkTheTree(n=> n.Expand());
        }

        public void WalkTheTree(Action<WebTree> processNodeFunction)
        {
            WalkTheTree(this, processNodeFunction);
        }


        private static void WalkTheTree(WebTree tree, Action<WebTree> processNodeFunction)
        {
            foreach (var node in tree)
            {
                processNodeFunction(node);
                WalkTheTree(node, processNodeFunction);
            }
        }
    }
}