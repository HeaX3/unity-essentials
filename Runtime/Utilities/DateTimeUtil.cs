using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Essentials
{
    public class DateTimeUtil
    {
        private static readonly Regex TwoDigitYearPattern = new Regex("^[0-9]{2}\\.[0-9]{2}\\.[0-9]{2}$");

        public static DateTime ConvertToLastDayOfMonth(DateTime date)
        {
            return new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
        }

        public static DateTime ParseDateString(string dateString, string format = "dd.MM.yyyy")
        {
            if (TwoDigitYearPattern.IsMatch(dateString))
            {
                format = "dd.MM.yy";
            }

            var result = !string.IsNullOrEmpty(dateString) &&
                         DateTime.TryParseExact(
                             dateString,
                             format,
                             CultureInfo.InvariantCulture,
                             DateTimeStyles.None,
                             out var d
                         )
                ? d
                : default;
            return format == "MM.yyyy" ? ConvertToLastDayOfMonth(result) : result;
        }

        public static bool TryParseUnixTimestamp(string dateString, out long timestamp)
        {
            return TryParseUnixTimestamp(dateString, "yyyy-MM-dd HH:mm:ss", out timestamp);
        }

        public static bool TryParseUnixTimestamp(string dateString, string format, out long timestamp)
        {
            var dateTime = ParseDateString(dateString, format);
            if (dateTime == default)
            {
                timestamp = default;
                return false;
            }

            timestamp = (long)dateTime.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            return true;
        }
    }
}