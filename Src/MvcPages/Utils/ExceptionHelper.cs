using System;

namespace Tellurium.MvcPages.Utils
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
            return $"{exception.Message}\r\n{innerExceptionMessage}".Trim();
        }

        public static T SwallowException<T>(Func<T> func, T defaultValue)
        {
            try
            {
                return func();
            }
            catch
            {
                return defaultValue;
            }
        }
    }
}