using System;

namespace Tellurium.MvcPages.SeleniumUtils
{
    public class WebElementNotFoundException: ApplicationException
    {
        public WebElementNotFoundException(string message, Exception innerException=null) 
            : base(message, innerException)
        {
        }
    }
}