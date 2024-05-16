namespace OnlineConsultationPlatform.Infrastructure
{
    public static class DateTimeExtensions
    {
        public static DateTime StartOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

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
    }
}
