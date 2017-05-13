using System;

namespace Tellurium.MvcPages.SeleniumUtils.Exceptions
{
    public class CannotInteractWithElementException:ApplicationException
    {
        public CannotInteractWithElementException(string message) : base(message)
        {
        }
    }
}