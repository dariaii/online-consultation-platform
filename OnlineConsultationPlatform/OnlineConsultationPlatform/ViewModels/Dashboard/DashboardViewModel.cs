using OnlineConsultationPlatform.Core.Services;
using OnlineConsultationPlatform.Data.Entities;

namespace OnlineConsultationPlatform.ViewModels
{
    public class DashboardViewModel
    {
        public ApplicationUser? User { get; set; }

        public DateTime StartOfMonth { get; set; } = DateTime.Now.ToFirstDayOfMonth();

        public List<Meeting>? Meetings { get; set; }

        List<Meeting>? _todayMeetings;
        public List<Meeting>? TodayMeetings
        {
            get
            {
                _todayMeetings ??= Meetings?.Where(x => x.MeetingDate != null && x.MeetingDate.Value.Date == DateTime.Now.Date && x.MeetingDate >= DateTime.Now).ToList();

                return _todayMeetings;
            }
        }

        List<Meeting>? _upcomingMeetings;
        public List<Meeting>? UpcomingMeetings
        {
            get
            {
                _upcomingMeetings ??= Meetings?.Where(x => x.MeetingDate != null && x.MeetingDate.Value.Date > DateTime.Now.Date).ToList();

                return _upcomingMeetings;
            }
        }

        List<Meeting>? _recentMeetings;
        public List<Meeting>? RecentMeetings
        {
            get
            {
                _recentMeetings ??= Meetings?.Where(x => x.MeetingDate != null && x.MeetingDate < DateTime.Now).ToList();

                return _recentMeetings;
            }
        }

        public List<ApplicationUser>? Mentors { get; set; }

        public CalendarViewModel Calendar { get; set; } = new();
    }
}