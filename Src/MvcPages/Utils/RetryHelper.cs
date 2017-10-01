using System;
using System.Threading;

namespace Tellurium.MvcPages.Utils
{
    internal static class RetryHelper
    {
        public static bool Retry(int numberOfRetries, Func<bool> action)
        {
            var currentNumberOfRetries = numberOfRetries;
            bool success;
            do
            {
                if (currentNumberOfRetries < numberOfRetries)
                {
                    Thread.Sleep(1000);
                }

                success = action.Invoke();
            } while (currentNumberOfRetries-- > 0 && success == false);

            return success;
        }


        public static RetryResult RetryWithExceptions(int numberOfRetries, Func<bool> action)
        {
            return RetryWithExceptions<Exception>(numberOfRetries, action);
        }  
        
        public static RetryResult RetryWithExceptions<T>(int numberOfRetries, Func<bool> action) where T:Exception
        {
            Exception lastException = null;
            var success = Retry(numberOfRetries, () =>
            {
                try
                {
                    return action();
                }
                catch (T ex)
                {
                    lastException = ex;
                    return false;
                }
            });
            return new RetryResult
            {
                Success = success,
                LastException = lastException
            };
        }
    }

    internal class RetryResult
    {
        public bool Success { get; set; }
        public Exception LastException { get; set; }
    }
}