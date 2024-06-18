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
        [Route("/admin/meetings")]
        public IActionResult GetMeetings(bool? showUnconfirmed = null, int? page = null, int? pageSize = null)
        {
            return View("Meetings", BuildMeetingsModel(showUnconfirmed, page, pageSize));
        }

        [HttpGet]
        public IActionResult FilterMeetings(bool? showUnconfirmed = null, int? page = null, int? pageSize = null)
        {
            return PartialView("_MeetingsTable", BuildMeetingsModel(showUnconfirmed, page, pageSize));
        }

        MeetingsGridViewModel BuildMeetingsModel(bool? showUnconfirmed = null, int? page = null, int? pageSize = null)
        {
            var viewModel = new MeetingsGridViewModel
            {
                Filter = new MeetingsFilterViewModel()
                {
                    ShowUnconfirmed = showUnconfirmed != null && Convert.ToBoolean(showUnconfirmed),
                    Page = page ?? 1,
                    PageSize = pageSize ?? PAGE_SIZE_MEETINGS
                }
            };

            viewModel.Data = _meetingService.GetMeetings(out var totalRecordsCount, viewModel.Filter.Page, viewModel.Filter.PageSize, true, viewModel.Filter.ShowUnconfirmed, false);

            viewModel.TotalRecordsCount = totalRecordsCount;

            return viewModel;
        }

        [HttpGet]
        [Route("/admin/users")]
        public IActionResult GetUsers(bool? showDeactivated = null, int? page = null, int? pageSize = null, string? userId = null)
        {
            if (userId != null)
            {
                _usersService.UpdateAccessStatus(userId);
            }

            return View("Users", BuildUsersModel(showDeactivated, page, pageSize));
        }

        [HttpGet]
        public IActionResult FilterUsers(bool? showDeactivated = null, int? page = null, int? pageSize = null)
        {
            return PartialView("_UserCards", BuildUsersModel(showDeactivated, page, pageSize));
        }

        [HttpPost]
        [Route("/admin/users/toggle-user")]
        public IActionResult ToggleAccountStatus(string userId, bool? showDeactivated = false, int? page = null, int? pageSize = null)
        {
            _usersService.UpdateAccessStatus(userId);

            return RedirectToAction(nameof(GetUsers), new { showDeactivated, page, pageSize });
        }

        UsersGridViewModel BuildUsersModel(bool? showDeactivated = null, int? page = null, int? pageSize = null)
        {
            var viewModel = new UsersGridViewModel
            {
                Filter = new UsersFilterViewModel()
                {
                    ShowDeactivated = showDeactivated != null && Convert.ToBoolean(showDeactivated),
                    Page = page ?? 1,
                    PageSize = pageSize ?? PAGE_SIZE_USERS
                }
            };

            viewModel.Data =
                _usersService.GetUsers(out var totalRecordsCount, viewModel.Filter.ShowDeactivated, viewModel.Filter.Page, viewModel.Filter.PageSize)
                    .Select(GetUserStatistics)
                    .ToList();

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
