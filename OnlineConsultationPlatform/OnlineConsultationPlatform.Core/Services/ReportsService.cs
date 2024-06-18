using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using OnlineConsultationPlatform.Core.Infrastructure.Enums;
using OnlineConsultationPlatform.Core.Infrastructure.Extensions;
using OnlineConsultationPlatform.Data.Entities;
using OnlineConsultationPlatform.Data.Repository;

namespace OnlineConsultationPlatform.Core.Services
{
    public interface IReportsService
    {
        List<Report> GetReports(out int totalRecordsCount, int? page = null, int? pageSize = null);

        bool SaveReport(IFormFile? file, Guid? meetingId, DateTime? meetingDate = null, string? studentName = null);
    }

    public class ReportsService(
        IRepository repository,
        IAuthenticationService authenticationService,
        IMeetingService meetingService,
        IFileService fileService,
        IStringLocalizer localizer,
        IEmailService emailService) : IReportsService
    {
        readonly IRepository _repository = repository;
        readonly IAuthenticationService _authenticationService = authenticationService;
        readonly IMeetingService _meetingService = meetingService;
        readonly IFileService _fileService = fileService;
        readonly IStringLocalizer _localizer = localizer;
        readonly IEmailService _emailService = emailService;

        readonly ApplicationUser _currentUser = authenticationService.CurrentUser();

        public List<Report> GetReports(out int totalRecordsCount, int? page = null, int? pageSize = null)
        {
            var query = _repository.SetNoTracking<Report>(nameof(Report.User), nameof(Report.Meeting));

            if (_authenticationService.IsUserInRole(Roles.Mentor))
            {
                query = query.Where(x => x.UserId == _currentUser.Id);
            }

            query = query.OrderBy(x => x.UploadDate).AsQueryable();

            totalRecordsCount = query.Count();

            if (page.HasValue && pageSize.HasValue)
            {
                query = query.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);
            }

            return query.ToList();
        }

        public bool SaveReport(IFormFile? file, Guid? meetingId, DateTime? meetingDate = null, string? studentName = null)
        {
            string fileName = GenerateFileName(file, meetingId, meetingDate, studentName);

            if (_fileService.UploadReportFile(file, fileName).Result)
            {
                var report = new Report
                {
                    UserId = _currentUser.Id,
                    MeetingId = meetingId,
                    ReportFilePath = fileName,
                    UploadDate = DateTime.Now
                };

                _repository.Add(report);

                SendReportUploadedEmail(report);

                return true;
            }

            return false;
        }

        string GenerateFileName(IFormFile? file, Guid? meetingId, DateTime? meetingDate = null, string? studentName = null)
        {
            if (meetingId != null)
            {
                var meeting = _meetingService.GetMeetingById((Guid)meetingId);

                return
                     $"{meeting?.MeetingDate.ToDayMonthYearNumbersOnly()}-{meeting?.User?.FirstName}-{meeting?.User?.LastName}{Path.GetExtension(file.FileName)}";
            }
            else if (meetingDate != null && studentName != null)
            {
                return
                    $"{meetingDate.ToDayMonthYearNumbersOnly()}-{studentName.Replace(" ", "-")}{Path.GetExtension(file.FileName)}";
            }

            return null;
        }

        void SendReportUploadedEmail(Report report)
        {
            var recievers = new List<string>();
            recievers.AddRange(_authenticationService.GetAdminUsersForEmailNotifications());

            var subject = _localizer["Email_ReportUploadedSubject"];
            var body = HttpUtility.HtmlEncode(_localizer["Email_ReportUploadedBody", _currentUser.Email, report.ReportFilePath, report.UploadDate.ToDayMonthYearWithHoursMinutesNumbersOnly()]);

            _emailService.SendEmail(recievers, subject, HttpUtility.HtmlDecode(body));
        }
    }
}
