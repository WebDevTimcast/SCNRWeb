using System;

namespace SCNRWeb.Helper
{
    public static class DateExtensions
    {
        public static TimeZoneInfo EasternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

        public static DateTime FromUTCToEastern(this DateTime timeUtc)
        {
            DateTime easternTime = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, EasternZone);

            return easternTime;
        }
    }
}
