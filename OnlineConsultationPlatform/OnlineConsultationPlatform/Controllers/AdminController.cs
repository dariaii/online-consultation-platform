using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineConsultationPlatform.ViewModels;
using OnlineConsultationPlatform.Core.Services;
using OnlineConsultationPlatform.Data.Entities;
using OnlineConsultationPlatform.Core.Infrastructure.Enums;

namespace OnlineConsultationPlatform.Controllers
{
    [Authorize(Roles = nameof(Roles.Admin))]
    public class AdminController(IUsersService usersService,
        IAuthenticationService authenticationService,
        IMeetingService meetingService,
        IConfiguration configuration) : Controller
    {
        private readonly IUsersService _usersService = usersService;
        private readonly IAuthenticationService _authenticationService = authenticationService;
        private readonly IMeetingService _meetingService = meetingService;

        private readonly int PAGE_SIZE_MEETINGS = configuration.GetValue<int>("PaginationSettings:MeetingsPageSize");
        private readonly int PAGE_SIZE_USERS = configuration.GetValue<int>("PaginationSettings:UsersPageSize");

        [HttpGet]
        [Route("/admin/users")]
        public IActionResult Users()
        {
            return View(GetAllUsers());
        }

        [HttpPost]
        [Route("/admin/users/toggle-user")]
        public IActionResult ToggleAccountStatus(string userId)
        {
            _usersService.UpdateAccessStatus(userId);

            return PartialView("_UserCard", GetUserStatistics(_authenticationService.GetUserById(userId)));
        }

        [HttpGet]
        public IActionResult GetFilteredUsers(UsersFilterViewModel filter)
        {
            return PartialView("_UserCards", GetAllUsers(filter));
        }

        [HttpGet]
        [Route("/admin/meetings")]
        public IActionResult Meetings()
        {
            return View(GetAllMeetings());
        }

        [HttpGet]
        public IActionResult GetFilteredMeetings(MeetingsFilterViewModel filter)
        {
            return PartialView("_MeetingsTable", GetAllMeetings(filter));
        }

        UsersGridViewModel GetAllUsers(UsersFilterViewModel filter = null)
        {
            var viewModel = new UsersGridViewModel
            {
                Filter = filter ?? new UsersFilterViewModel() { Page = 1, PageSize = PAGE_SIZE_USERS }
            };

            viewModel.Data = 
                _usersService.GetUsers(out var totalRecordsCount, viewModel.Filter.ShowDeactivated, viewModel.Filter.Page, viewModel.Filter.PageSize)
                    .Select(user => GetUserStatistics(user))
                    .ToList();

            viewModel.TotalRecordsCount = totalRecordsCount;

            return viewModel;
        }

        MeetingsGridViewModel GetAllMeetings(MeetingsFilterViewModel filter = null)
        {
            var viewModel = new MeetingsGridViewModel
            {
                Filter = filter ?? new MeetingsFilterViewModel() { Page = 1, PageSize = PAGE_SIZE_MEETINGS }
            };

            viewModel.Data = _meetingService.GetMeetings(out var totalRecordsCount, viewModel.Filter.Page, viewModel.Filter.PageSize, true, viewModel.Filter.ShowUnconfirmed, false);
            
            viewModel.TotalRecordsCount = totalRecordsCount;

            return viewModel;
        }

        UserStatisticsViewModel GetUserStatistics(ApplicationUser user)
        {
            var statistics = _usersService.GetUserStatistics(user.Id);

            return new UserStatisticsViewModel
            {
                User = user,
                NumberOfMeetingsBooked = statistics.BookedMeetings,
                NumberOfRatings = statistics.NumberOfRatings,
                AverageRating = statistics.AverageRating
            };
        }
    }
}
