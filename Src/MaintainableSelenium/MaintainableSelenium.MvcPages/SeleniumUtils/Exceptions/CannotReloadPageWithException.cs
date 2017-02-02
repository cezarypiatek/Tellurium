using System;

namespace MaintainableSelenium.MvcPages.SeleniumUtils.Exceptions
{
    public class CannotReloadPageWithException:ApplicationException
    {
        public CannotReloadPageWithException():base("Cannot reload page with given action")
        {
        }
    }
}