using System;
using OpenQA.Selenium;

namespace Tellurium.MvcPages.SeleniumUtils.Exceptions
{
    public class CannotFindElementByException: WebElementNotFoundException
    {
        public CannotFindElementByException(By @by, ISearchContext context, Exception originalException = null)
            :base($"Cannot find element {by} inside {context.GetElementDescription()}", originalException)
        {
        }
    }
}