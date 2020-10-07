using System;
using System.Threading;

namespace Tellurium.StableElements
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
    }
}