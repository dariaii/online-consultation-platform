using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineConsultationPlatform.ViewModels;
using OnlineConsultationPlatform.Core.Services;

namespace OnlineConsultationPlatform.Controllers
{
    [Authorize]
    public class DashboardController(IAuthenticationService authenticationService, IMeetingService meetingService, IUsersService usersService) : Controller
    {
        private readonly IAuthenticationService _authenticationService = authenticationService;
        private readonly IMeetingService _meetingService = meetingService;
        private readonly IUsersService _usersService = usersService;

        [HttpGet]
        [Route("/")]
        public IActionResult Index()
        {
            return View(new DashboardViewModel
            {
                User = _authenticationService.CurrentUser(),
                Meetings = _meetingService.GetMeetings(out _),
                Mentors = _usersService.GetMentors(includeRelatedMentors: true),
                Calendar = BuildCalendarViewModel(DateTime.Now.ToFirstDayOfMonth())
            });
        }

        [HttpGet]
        [Route("/dashboard/calendar")]
        public IActionResult Calendar(DateTime date)
        {
            return PartialView("_Calendar", BuildCalendarViewModel(date));
        }

        CalendarViewModel BuildCalendarViewModel(DateTime date)
        {
            return
                new CalendarViewModel
                {
                    Date = date,
                    Meetings = _meetingService.GetMeetingsByMonth(date.Month)
                };
        }
    }
}
