using System;

namespace Tellurium.VisualAssertions.Screenshots.Utils
{
    static class DateTimeExtensions
    {
        public static DateTime TrimToMiliseconds(this DateTime dateTime)
        {
            return Trim(dateTime, TimeSpan.TicksPerMillisecond);
        }

        static DateTime Trim(DateTime date, long roundTicks)
        {
            return new DateTime(date.Ticks - date.Ticks % roundTicks);
        }
    }
}