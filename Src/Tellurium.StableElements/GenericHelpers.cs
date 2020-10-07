using System;
using OpenQA.Selenium;

namespace Tellurium.MvcPages.SeleniumUtils
{
    internal static class GenericHelpers
    {
        public static TInterface As<TInterface>(this IWebElement element) where TInterface : class
        {
            var typed = element as TInterface;
            if (typed == null)
            {
                var errorMessage = $"Underlying element does not support this operation. It should implement {typeof(TInterface).FullName} interface";
                throw new NotSupportedException(errorMessage);
            }
            return typed;
        }
    }
}