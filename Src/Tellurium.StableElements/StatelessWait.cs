using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Tellurium.MvcPages.SeleniumUtils
{
    internal class StatelessWait : DefaultWait<object>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:OpenQA.Selenium.Support.UI.WebDriverWait" /> class.
        /// </summary>
        /// <param name="driver">The WebDriver instance used to wait.</param>
        /// <param name="timeout">The timeout value indicating how long to wait for the condition.</param>
        public StatelessWait(TimeSpan timeout)
            : this((IClock)new SystemClock(), timeout, DefaultSleepTimeout)
        {
        }

        private static TimeSpan DefaultSleepTimeout { get; } = TimeSpan.FromMilliseconds(500.0);
        
        private StatelessWait(
            IClock clock,
            TimeSpan timeout,
            TimeSpan sleepInterval)
            : base(new object(), clock)
        {
            this.Timeout = timeout;
            this.PollingInterval = sleepInterval;
            this.IgnoreExceptionTypes(new Type[1]
            {
                typeof (NotFoundException)
            });
        }
    }
}