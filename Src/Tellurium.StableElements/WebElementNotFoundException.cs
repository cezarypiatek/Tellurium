using System;

namespace Tellurium.MvcPages.SeleniumUtils
{
    public class WebElementNotFoundException: Exception
    {
        public WebElementNotFoundException(string message, Exception innerException=null) 
            : base(message, innerException)
        {
        }
    }
}