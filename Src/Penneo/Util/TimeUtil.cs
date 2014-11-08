using System;

namespace Penneo.Util
{
    /// <summary>
    /// Time utilities
    /// </summary>
    internal static class TimeUtil
    {
        /// <summary>
        /// Converts a unix timestamp to a DateTime
        /// </summary>
        public static DateTime FromUnixTime(long unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unixTime);
        }

        /// <summary>
        /// Converts a datetime to unix time
        /// </summary>
        public static long ToUnixTime(DateTime dt)
        {
            var timeSpan = (dt - new DateTime(1970, 1, 1, 0, 0, 0));
            return (long) timeSpan.TotalSeconds;
        }
    }
}