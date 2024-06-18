namespace OnlineConsultationPlatform.Infrastructure
{
    public static class DateTimeExtensions
    {
        public static DateTime EndOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1).AddMonths(1).AddDays(-1);
        }

        public static DateTime StartOfWeek(this DateTime date, DayOfWeek startDayOfWeek)
        {
            int diff = date.DayOfWeek - startDayOfWeek;

            if (diff < 0)
            {
                diff += 7;
            }
            return date.AddDays(-1 * diff).Date;
        }

        public static DateTime ToFirstDayOfMonth(this DateTime time)
        {
            return new DateTime(time.Year, time.Month, 1);
        }

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
    }
}
