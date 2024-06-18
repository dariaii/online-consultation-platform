using OnlineConsultationPlatform.Data.Entities;
using OnlineConsultationPlatform.Data.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using System.Linq.Expressions;
using System.Web;
using OnlineConsultationPlatform.Core.Infrastructure.Enums;
using OnlineConsultationPlatform.Core.Infrastructure.Extensions;

namespace OnlineConsultationPlatform.Core.Services
{
    public interface IMeetingService
    {
        List<Meeting> GetMeetings(
            out int totalRecordsCount,
            int? page = null,
            int? pageSize = null,
            bool hasAssignedMentor = false,
            bool showUnconfirmed = false,
            bool sortByAsc = true);

        List<Meeting> GetMeetingsWithoutReports();

        List<Meeting> GetMeetingsByMonth(int month);

        List<Meeting> GetPendingMeetings(out int totalRecordsCount, int? page = null, int? pageSize = null);

        List<Meeting> GetMeetingsByMentorForDates(string mentorId, DateTime firstDateOption, DateTime secondDateOption);

        Meeting? GetMeetingById(Guid meetingId);

        bool CreateMeeting(string? mentorId,
            DateTime firstDateOption,
            DateTime secondDateOption,
            string? subject,
            string? firstQuestion,
            string? secondQuestion,
            string? thirdQuestion,
            string? extraInformation);

        bool AcceptMeeting(Guid meetingId, DateTime meetingDate);

        bool DeclineMeeting(Guid meetingId);

        bool AssignMentor(Guid meetingId, string mentorId);

        void LeaveFeedback(int? rating, Guid meetingId, string? emailSubject, string? emailBody);
    }

    public class MeetingService : IMeetingService
    {
        readonly IRepository _repository;
        readonly IConfiguration _configuration;
        readonly IAuthenticationService _authenticationService;
        readonly IEmailService _emailService;
        readonly IStringLocalizer _localizer;

        readonly int ADMIN_DAYS_INTERVAL;
        readonly int DEFAULT_DAYS_INTERVAL;

        readonly ApplicationUser _currentUser;

        public MeetingService(IRepository repository,
            IConfiguration configuration,
            IAuthenticationService authenticationService,
            IEmailService emailService,
            IStringLocalizer localizer)
        {
            _repository = repository;
            _configuration = configuration;
            _authenticationService = authenticationService;
            _emailService = emailService;
            _localizer = localizer;

            ADMIN_DAYS_INTERVAL = Convert.ToInt32(_configuration["DashboardSettings:AdminDaysInterval"]);
            DEFAULT_DAYS_INTERVAL = Convert.ToInt32(_configuration["DashboardSettings:DefaultDaysInterval"]);

            _currentUser = _authenticationService.CurrentUser();
        }

        public List<Meeting> GetMeetings(out int totalRecordsCount,
            int? page = null,
            int? pageSize = null,
            bool hasAssignedMentor = false,
            bool showUnconfirmed = false,
            bool sortByAsc = true)
        {
            IQueryable<Meeting> meetings =
                _authenticationService.IsUserInRole(Roles.Admin) ? FindMeetingsForAdmin() : FindMeetingsForMentorOrStudent();

            if (hasAssignedMentor)
            {
                meetings = meetings.Where(x => x.MentorId != null);
            }

            if (!showUnconfirmed)
            {
                meetings = meetings.Where(x => x.MeetingDate != null);
            }

            meetings = sortByAsc ? meetings.OrderBy(x => x.MeetingDate).AsQueryable() : meetings.OrderByDescending(x => x.MeetingDate).AsQueryable();

            totalRecordsCount = meetings.Count();

            if (page.HasValue && pageSize.HasValue)
            {
                meetings = meetings.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);
            }

            return [.. meetings];
        }

        IQueryable<Meeting> FindMeetingsForAdmin()
        {
            var firstDayFromInterval = DateTime.Now.AddDays(-ADMIN_DAYS_INTERVAL);
            var lastDayFromInterval = DateTime.Now.AddDays(ADMIN_DAYS_INTERVAL);

            var meetings = _repository
                .SetNoTracking<Meeting>(nameof(Meeting.User), nameof(Meeting.Mentor))
                .Where(WithIntervalsPredicate<Meeting>(firstDayFromInterval, lastDayFromInterval));

            return meetings;
        }

        IQueryable<Meeting> FindMeetingsForMentorOrStudent()
        {
            var firstDayFromInterval = DateTime.Now.AddDays(-DEFAULT_DAYS_INTERVAL);
            var lastDayFromInterval = DateTime.Now.AddDays(DEFAULT_DAYS_INTERVAL);

            var meetings = _repository
                .SetNoTracking<Meeting>(nameof(Meeting.User), nameof(Meeting.Mentor))
                .Where(WithIntervalsPredicate<Meeting>(firstDayFromInterval, lastDayFromInterval))
                .AsQueryable()
                .Where(x => x.UserId == _currentUser.Id || x.MentorId == _currentUser.Id);

            return meetings;
        }

        Expression<Func<T, bool>> WithIntervalsPredicate<T>(DateTime firstDayFromInterval, DateTime lastDayFromInterval) where T : Meeting
        {
            return
                x =>
                    (x.MeetingDate > firstDayFromInterval && x.MeetingDate < lastDayFromInterval)
                    ||
                    (x.FirstDateOption > firstDayFromInterval && x.FirstDateOption < lastDayFromInterval)
                    ||
                    (x.SecondDateOption > firstDayFromInterval && x.SecondDateOption < lastDayFromInterval);
        }

        public List<Meeting> GetMeetingsWithoutReports()
        {
            return
                _repository
                    .SetNoTracking<Meeting>(nameof(Meeting.User), nameof(Meeting.Reports))
                    .Where(x => x.MentorId == _currentUser.Id && x.MeetingDate != null && x.Reports.Count == 0)
                    .OrderBy(x => x.MeetingDate)
                    .ToList();
        }

        public Meeting? GetMeetingById(Guid meetingId)
        {
            return
                _repository.SetNoTracking<Meeting>(nameof(Meeting.User)).FirstOrDefault(x => x.Id == meetingId);
        }

        public void LeaveFeedback(int? rating, Guid meetingId, string? emailSubject, string? emailBody)
        {
            var meeting = _repository.Set<Meeting>().FirstOrDefault(x => x.Id == meetingId);
            if (meeting != null)
            {
                meeting.Rating = rating;

                _repository.Update(meeting);

                if (!string.IsNullOrEmpty(emailSubject) && !string.IsNullOrEmpty(emailBody))
                {
                    SendFeedbackEmail(emailSubject, emailBody);
                }
            }
        }

        public List<Meeting> GetMeetingsByMonth(int month)
        {
            return
                _repository
                    .SetNoTracking<Meeting>(nameof(Meeting.Mentor))
                    .Where(x =>
                        (x.UserId == _currentUser.Id || x.MentorId == _currentUser.Id)
                        && !x.IsDeclined
                        &&
                        (
                            (x.MeetingDate.HasValue && x.MeetingDate.Value.Month == month)
                            ||
                            x.FirstDateOption.Month == month
                            ||
                            x.SecondDateOption.Month == month
                        ))
                    .ToList();
        }

        public bool CreateMeeting(
            string? mentorId,
            DateTime firstDateOption,
            DateTime secondDateOption,
            string? subject,
            string? firstQuestion,
            string? secondQuestion,
            string? thirdQuestion,
            string? extraInformation)
        {
            //check if mentor selected is available
            if (mentorId != null && GetMeetingsByMentorForDates(mentorId, firstDateOption, secondDateOption).Count != 0)
            {
                return false;
            }

            var newMeeting = new Meeting
            {
                UserId = _authenticationService.CurrentUser()?.Id,
                MentorId = mentorId,
                CreatedDate = DateTime.Now,
                MeetingDate = null,
                FirstDateOption = firstDateOption,
                SecondDateOption = secondDateOption,
                Subject = subject,
                FirstQuestion = firstQuestion,
                SecondQuestion = secondQuestion,
                ThirdQuestion = thirdQuestion,
                ExtraInformation = extraInformation
            };

            _repository.Add(newMeeting);

            SendRequestConfirmationEmail(mentorId);

            return true;
        }

        public List<Meeting> GetPendingMeetings(out int totalRecordsCount, int? page = null, int? pageSize = null)
        {
            var isAdmin = _authenticationService.IsUserInRole(Roles.Admin);

            var query = _repository
                    .SetNoTracking<Meeting>(nameof(Meeting.User))
                    .Where(x => x.MeetingDate == null && !x.IsDeclined);

            if (_authenticationService.IsUserInRole(Roles.Admin))
            {
                query = query.Where(x => x.MentorId == null);
            }
            else
            {
                query = query.Where(x => x.MentorId == _currentUser.Id);
            }

            totalRecordsCount = query.Count();

            query = query.OrderByDescending(x => x.CreatedDate.Date).AsQueryable();

            if (page.HasValue && pageSize.HasValue)
            {
                query = query.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);
            }

            return query.ToList();
        }

        public bool AcceptMeeting(Guid meetingId, DateTime meetingDate)
        {
            var meeting = _repository.Set<Meeting>().FirstOrDefault(x => x.Id == meetingId);

            if (meeting != null)
            {
                meeting.MeetingDate = meetingDate;
                meeting.MeetingUrl = _configuration["JitsiMeetingUrl"] + meetingId;

                _repository.Update(meeting);

                SendRequestAcceptedEmail(meeting);

                return true;
            }

            return false;
        }

        public bool DeclineMeeting(Guid meetingId)
        {
            var meeting = _repository.Set<Meeting>().FirstOrDefault(x => x.Id == meetingId);

            if (meeting != null)
            {
                meeting.IsDeclined = true;

                _repository.Update(meeting);

                SendRequestDeclinedEmail(meeting);

                return true;
            }

            return false;
        }

        public bool AssignMentor(Guid meetingId, string mentorId)
        {
            var meeting = _repository.Set<Meeting>().FirstOrDefault(x => x.Id == meetingId);

            if (meeting != null && GetMeetingsByMentorForDates(mentorId, meeting.FirstDateOption, meeting.SecondDateOption).Count == 0)
            {
                meeting.MentorId = mentorId;

                _repository.Update(meeting);

                SendRequestConfirmationEmail(mentorId, isAssignedByAdminUser: true);

                return true;
            }

            return false;
        }

        public List<Meeting> GetMeetingsByMentorForDates(string mentorId, DateTime firstDateOption, DateTime secondDateOption)
        {
            var meetings = _repository
               .SetNoTracking<Meeting>()
               .Where(m =>
                       m.MentorId == mentorId
                       &&
                       (
                            m.MeetingDate == null
                            ? (
                                m.FirstDateOption == firstDateOption || m.FirstDateOption == secondDateOption
                                || m.SecondDateOption == firstDateOption || m.SecondDateOption == secondDateOption
                            )
                            : (
                                m.MeetingDate == firstDateOption || m.MeetingDate == secondDateOption
                            )
                       )
                    )
               .ToList();

            return meetings;
        }

        void SendFeedbackEmail(string subjectFromUser, string bodyFromUser)
        {
            var recievers = _authenticationService.GetAdminUsersForEmailNotifications();

            var subject = _localizer["Email_FeedbackSubject", subjectFromUser];
            var body = HttpUtility.HtmlEncode(_localizer["Email_FeedbackBody", _currentUser.Email, bodyFromUser]);

            _emailService.SendEmail(recievers, subject, HttpUtility.HtmlDecode(body));
        }

        void SendRequestConfirmationEmail(string? mentorId, bool isAssignedByAdminUser = false)
        {
            var recievers = new List<string>();

            if (mentorId != null)
            {
                recievers.Add(_authenticationService.GetUserById(mentorId).Email);
            }

            if (!isAssignedByAdminUser)
            {
                recievers.AddRange(_authenticationService.GetAdminUsersForEmailNotifications());
            }

            var subject = _localizer["Email_MeetingRequestConfirmationSubject"];
            var body = HttpUtility.HtmlEncode(_localizer["Email_MeetingRequestConfirmationBody", _currentUser.Email]);

            _emailService.SendEmail(recievers, subject, HttpUtility.HtmlDecode(body));
        }

        void SendRequestAcceptedEmail(Meeting meeting)
        {
            var recievers = new List<string>();
            recievers.AddRange(_authenticationService.GetAdminUsersForEmailNotifications());

            var subject = _localizer["Email_MeetingRequestAcceptedSubject"];
            var body = HttpUtility.HtmlEncode(_localizer["Email_MeetingRequestAcceptedBody", _currentUser.Email, meeting.MeetingDate.ToDayMonthYearWithHoursMinutesNumbersOnly(), meeting.Subject]);

            _emailService.SendEmail(recievers, subject, HttpUtility.HtmlDecode(body));
        }

        void SendRequestDeclinedEmail(Meeting meeting)
        {
            var recievers = new List<string>();
            recievers.AddRange(_authenticationService.GetAdminUsersForEmailNotifications());

            var subject = _localizer["Email_MeetingRequestDeclinedSubject"];
            var body = HttpUtility.HtmlEncode(_localizer["Email_MeetingRequestDeclinedBody", _currentUser.Email, meeting.FirstDateOption.ToDayMonthYearWithHoursMinutesNumbersOnly(), meeting.SecondDateOption.ToDayMonthYearWithHoursMinutesNumbersOnly(), meeting.Subject]);

            _emailService.SendEmail(recievers, subject, HttpUtility.HtmlDecode(body));
        }
    }
}
