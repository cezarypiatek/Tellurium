using System;

namespace MaintainableSelenium.Toolbox.Infrastructure
{
    public static class ExceptionHelper
    {
        public static string GetFullExceptionMessage(this Exception exception)
        {
            if (exception == null)
            {
                return string.Empty;
            }
            
            var innerExceptionMessage = exception.InnerException.GetFullExceptionMessage();
            return string.Format("{0}\r\n{1}", exception.Message, innerExceptionMessage).Trim();
        }
    }
}