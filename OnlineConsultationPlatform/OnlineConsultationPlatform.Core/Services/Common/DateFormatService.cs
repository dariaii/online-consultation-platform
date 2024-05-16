namespace OnlineConsultationPlatform.Core.Services
{
    public static class DateFormatService
    {
        public static string ToDayMonthYear(this DateTime? time, string format = "dd MMMM yyyy")
        {
            if (time.HasValue)
            {
                return time.Value.ToString(format);
            }

            return string.Empty;
        }

        public static string ToDayMonth(this DateTime? time, string format = "dd MMMM")
        {
            if (time.HasValue)
            {
                return time.Value.ToString(format);
            }

            return string.Empty;
        }

        public static string ToHourMinute(this DateTime? time, string format = "HH:mm")
        {
            if (time.HasValue)
            {
                return time.Value.ToString(format);
            }

            return string.Empty;
        }

        public static string ToDayOfWeek(this DateTime? time, string format = "dddd")
        {
            if (time.HasValue)
            {
                return time.Value.ToString(format);
            }

            return string.Empty;
        }

        public static string ToMonthYear(this DateTime? time, string format = "MMMM yyyy")
        {
            if (time.HasValue)
            {
                return time.Value.ToString(format);
            }

            return string.Empty;
        }

        public static string ToDayMonthYearNumbersOnly(this DateTime? time, string format = "dd-MM-yyyy")
        {
            if (time.HasValue)
            {
                return time.Value.ToString(format);
            }

            return string.Empty;
        }

        public static string ToDayMonthYearWithHoursMinutesNumbersOnly(this DateTime time, string format = "dd-MM-yyyy hh:mm")
        {
            if (time != null)
            {
                return time.ToString(format);
            }

            return string.Empty;
        }

        public static string ToDayMonthYearWithHoursMinutesNumbersOnly(this DateTime? time, string format = "dd-MM-yyyy hh:mm")
        {
            if (time.HasValue)
            {
                return time.Value.ToString(format);
            }

            return string.Empty;
        }

        public static DateTime ToFirstDayOfMonth(this DateTime time)
        {
            return new DateTime(time.Year, time.Month, 1);
        }
    }
}