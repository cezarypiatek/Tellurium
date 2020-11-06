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
            return $"{innerExceptionMessage}\r\n{exception.GetType().FullName}: {exception.Message}\r\n{exception.StackTrace}".Trim();
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

        public static void SwallowException(Action func)
        {
            try
            {
                func();
            }
            catch
            {
                // Code block intentionally empty
            }
        }
    }
}