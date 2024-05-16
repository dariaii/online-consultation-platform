using OnlineConsultationPlatform.Infrastructure;
using OnlineConsultationPlatform.Data.Entities;

namespace OnlineConsultationPlatform.ViewModels
{
    public class CalendarViewModel
    {
        public DateTime Date { get; set; }

        public List<Meeting> Meetings { get; set; } = [];

        public DateTime EndOfPreviousMonth
        {
            get
            {
                return Date.AddMonths(-1).EndOfMonth();
            }
        }

        public DateTime StartOfLastWeekInPreviousMonth
        {
            get
            {
                return EndOfPreviousMonth.StartOfWeek(DayOfWeek.Monday);
            }
        }

        public DateTime EndOfCurrentMonth
        {
            get
            {
                return Date.EndOfMonth();
            }
        }
    }
}
