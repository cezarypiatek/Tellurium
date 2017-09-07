using System;
using OpenQA.Selenium;

namespace Tellurium.MvcPages.SeleniumUtils.Exceptions
{
    public class CannotFindElementByException: WebElementNotFoundException
    {
        public CannotFindElementByException(By by, Exception originalException=null)
            :base($"Cannot find element {by}", originalException)
        {
        }
    }
}