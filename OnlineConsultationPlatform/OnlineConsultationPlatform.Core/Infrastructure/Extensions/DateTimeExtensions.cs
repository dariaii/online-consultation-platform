namespace OnlineConsultationPlatform.Core.Infrastructure.Extensions
{
    public static class DateTimeExtensions
    {
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
    }
}