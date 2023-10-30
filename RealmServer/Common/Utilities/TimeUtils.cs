using System;

namespace Common.Utilities
{
    public static class TimeUtils
    {
        public static int ToUnixTimestamp(this DateTime dateTime)
            => (int)(dateTime - new DateTime(1970, 1, 1)).TotalSeconds;

        public static DateTime FromUnixTimestamp(int time)
        {
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(time).ToLocalTime();
            return dateTime;
        }

        // https://www.codeproject.com/Articles/770323/How-to-Convert-a-Date-Time-to-X-minutes-ago-in-Csh
        public static string TimeAgo(DateTime dt)
        {
            TimeSpan span = DateTime.Now - dt;
            if (span.Days > 365)
            {
                int years = span.Days / 365;
                if (span.Days % 365 != 0)
                    years += 1;
                return $"{years} {(years == 1 ? "year" : "years")} ago";
            }
            if (span.Days > 30)
            {
                int months = span.Days / 30;
                if (span.Days % 31 != 0)
                    months += 1;
                return $"{months} {(months == 1 ? "month" : "months")} ago";
            }
            if (span.Days > 0)
                return $"{span.Days} {(span.Days == 1 ? "day" : "days")} ago";
            if (span.Hours > 0)
                return $"{span.Hours} {(span.Hours == 1 ? "hour" : "hours")} ago";
            if (span.Minutes > 0)
                return $"{span.Minutes} {(span.Minutes == 1 ? "minute" : "minutes")} ago";
            if (span.Seconds > 5)
                return $"{span.Seconds} seconds ago";
            if (span.Seconds <= 5)
                return "just now";
            return string.Empty;
        }
    }
}
