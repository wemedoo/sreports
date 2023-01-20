using System;
using System.Linq;
using TimeZoneConverter;

namespace sReportsV2.Common.Extensions
{
    public static class DateTimeExtension
    {
        private static readonly string defaultTimezone = "Europe/London";
        public static DateTime? ToTimeZonedDateTime(this DateTime? dateTime, string timeZone)
        {
            if (dateTime.HasValue) 
            {
                return ToTimeZoned(dateTime.Value, timeZone);
            } 
            else
            {
                return null;
            }
        }

        public static DateTime ToTimeZoned(this DateTime dateTime, string timeZone)
        {
            TimeZoneInfo windowsTZ = TimeZoneInfo.FindSystemTimeZoneById(TZConvert.IanaToWindows(timeZone ?? defaultTimezone));
            DateTime utc = dateTime.ToUniversalTime();
            return TimeZoneInfo.ConvertTimeFromUtc(utc, windowsTZ);
        }

        public static DateTime AppendDays(this DateTime date, int dayNumber)
        {
            return date.AddDays(GetOffset(dayNumber));
        }

        private static int GetOffset(int dayNumber)
        {
            int sign = dayNumber > 0 ? -1 : 0;
            return dayNumber + 1 * sign;
        }

        public static string ToTimeZonedDateTime(this DateTime? dateTime, string timeZone, string dateFormat)
        {
            return dateTime.HasValue ? ToTimeZoned(dateTime.Value, timeZone, dateFormat) : string.Empty;
        }

        public static string ToTimeZoned(this DateTime dateTime, string timeZone, string dateFormat)
        {
            return ToTimeZoned(dateTime, timeZone).GetDateTimeDisplay(dateFormat);
        }

        public static string GetTimePart(this DateTime date)
        {
            string fullTimePart = date.ToString("s").Split('T')[1];
            int secondsSeparatorIndex = fullTimePart.LastIndexOf(':');
            return fullTimePart.Substring(0, secondsSeparatorIndex);
        }

        public static string GetDateTimeDisplay(this DateTime date, string dateFormat)
        {
            return $"{date.ToString(dateFormat)} {date.GetTimePart()}";
        }

        public static string RenderDate(this string dateTimeValue)
        {
            if (!string.IsNullOrWhiteSpace(dateTimeValue))
            {
                string[] dateTimeParts = dateTimeValue.Split('T');
                string datePart = dateTimeParts[0];
                return HandleValueDuplication(datePart);
            }
            else
            {
                return "";
            }
        }

        public static string RenderTime(this string dateTimeValue)
        {
            if (!string.IsNullOrWhiteSpace(dateTimeValue))
            {
                string[] dateTimeParts = dateTimeValue.Split('T');
                string timePart = dateTimeParts.Length == 2 ? dateTimeParts[1] : "";
                return HandleValueDuplication(timePart);
            }
            else
            {
                return "";
            }
        }

        private static string HandleValueDuplication(string dateTimeValue)
        {
            return dateTimeValue.Contains(',') ? dateTimeValue.Split(',')[0] : dateTimeValue;
        }
    }
}