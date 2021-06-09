using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TimeZoneConverter;

namespace sReportsV2.Common.Extensions
{
    public static class DateTimeExtension
    {
        public static DateTime? ToTimeZonedDateTime(this DateTime? dateTime, string timeZone)
        {
            TimeZoneInfo windowsTZ = TimeZoneInfo.FindSystemTimeZoneById(TZConvert.IanaToWindows(timeZone));
            DateTime utc = dateTime.Value.ToUniversalTime();
            return TimeZoneInfo.ConvertTimeFromUtc(utc, windowsTZ);
        }

        public static DateTime ToTimeZoned(this DateTime dateTime, string timeZone)
        {
            TimeZoneInfo windowsTZ = TimeZoneInfo.FindSystemTimeZoneById(TZConvert.IanaToWindows(timeZone));
            DateTime utc = dateTime.ToUniversalTime();
            return TimeZoneInfo.ConvertTimeFromUtc(utc, windowsTZ);
        }


    }
}