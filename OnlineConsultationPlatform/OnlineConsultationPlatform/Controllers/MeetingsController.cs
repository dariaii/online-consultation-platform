using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using OnlineConsultationPlatform.Core.Services;
using OnlineConsultationPlatform.ViewModels;
using OnlineConsultationPlatform.Core.Infrastructure.Enums;

namespace OnlineConsultationPlatform.Controllers
{
    [Authorize]
    public class MeetingsController(
        IMeetingService meetingService,
        IUsersService usersService,
        IStringLocalizer localizer,
        IConfiguration configuration) : Controller
    {
        readonly IMeetingService _meetingService = meetingService;
        readonly IUsersService _usersService = usersService;
        readonly IStringLocalizer _localizer = localizer;

        readonly int PAGE_SIZE = configuration.GetValue<int>("PaginationSettings:RequestsPageSize");

        [Authorize(Roles = nameof(Roles.Teacher))]
        [HttpGet]
        [Route("/meetings")]
        public IActionResult Index()
        {
            return View(BuildModel());
        }

        [Authorize(Roles = nameof(Roles.Teacher))]
        [HttpPost]
        [Route("/meetings/new")]
        public IActionResult CreateMeetingRequest(MeetingRequestViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = _meetingService.CreateMeeting(model.ChosenMentorId,
                    model.FirstDateOption,
                    model.SecondDateOption,
                    model.Subject,
                    model.FirstQuestion,
                    model.SecondQuestion,
                    model.ThirdQuestion,
                    model.ExtraInformation);

                if (result)
                {
                    return RedirectToAction(nameof(RequestConfirmed));
                }
            }

            TempData["UnsuccessfulBookAMeeting"] = _localizer["Meeting_UnsuccessfulBookAMeeting"].ToString();

            model.Mentors = _usersService.GetMentors();


            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = nameof(Roles.Teacher))]
        [HttpGet]
        [Route("/meetings/confirmed")]
        public IActionResult RequestConfirmed()
        {
            return View("MeetingRequestConfirmed");
        }

        [Authorize(Roles = nameof(Roles.Teacher))]
        [HttpGet]
        [Route("/meetings/feedback")]
        public IActionResult GetFeedbackForm(Guid meetingId)
        {
            var meeting = _meetingService.GetMeetingById(meetingId);

            return View("FeedbackForm", new FeedbackViewModel
            {
                MeetingId = meetingId,
                Rating = meeting.Rating
            });
        }

        [Authorize(Roles = nameof(Roles.Teacher))]
        [HttpPost]
        [Route("/meetings/feedback")]
        public IActionResult LeaveFeedback(FeedbackViewModel model)
        {
            if (ModelState.IsValid)
            {
                _meetingService.LeaveFeedback(model.Rating, model.MeetingId, model.FeedbackSubject, model.FeedbackText);

                TempData[$"SubmittedFeedback{model.MeetingId}"] = _localizer["Feedback_SuccessfulSubmit"].ToString();

                return RedirectToAction(nameof(DashboardController.Index), "Dashboard");
            }

            return View("FeedbackForm", model);
        }

        MeetingRequestViewModel BuildModel()
        {
            return new MeetingRequestViewModel
            {
                Mentors = _usersService.GetMentors()
            };
        }

        //Requests

        [Authorize(Roles = nameof(Roles.Admin) + "," + nameof(Roles.Mentor))]
        [HttpGet]
        [Route("/meetings/requests")]
        public IActionResult MeetingRequests()
        {
            return View("MeetingRequests", BuildMeetingRequestModel());
        }

        [Authorize(Roles = nameof(Roles.Admin) + "," + nameof(Roles.Mentor))]
        [HttpGet]
        public IActionResult GetMeetingRequestsNextPage(MeetingsFilterViewModel filter)
        {
            return PartialView("_MeetingRequestsList", BuildMeetingRequestModel(filter));
        }

        [Authorize(Roles = nameof(Roles.Admin) + "," + nameof(Roles.Mentor))]
        [HttpGet]
        [Route("/meetings/requests/details")]
        public IActionResult ShowMeetingRequestDetails(Guid meetingId)
        {
            return View("Details", _meetingService.GetMeetingById(meetingId));
        }

        [Authorize(Roles = nameof(Roles.Admin) + "," + nameof(Roles.Mentor))]
        [HttpPost]
        public IActionResult AcceptMeetingRequest(MeetingConfirmationViewModel model)
        {
            if (ModelState.IsValid)
            {
                _meetingService.AcceptMeeting(model.MeetingId.Value, model.Date.Value);

                TempData["RequestSuccess"] = _localizer["Requests_SuccessfulRequestAccept"].ToString();

                return Ok();
            }

            TempData["MessageFailure"] = _localizer["Requests_FailedRequestConfirmation"].ToString();

            return PartialView("_ConfirmMeeting", _meetingService.GetMeetingById(model.MeetingId.Value));
        }

        [Authorize(Roles = nameof(Roles.Admin) + "," + nameof(Roles.Mentor))]
        [HttpPost]
        public IActionResult DeclineMeetingRequest(Guid meetingId)
        {
            _meetingService.DeclineMeeting(meetingId);

            TempData["RequestSuccess"] = _localizer["Requests_SuccessfulRequestDecline"].ToString();

            return RedirectToAction(nameof(MeetingRequests));
        }

        [Authorize(Roles = nameof(Roles.Admin) + "," + nameof(Roles.Mentor))]
        [HttpPost]
        [Route("/meetings/requests/assign-mentor")]
        public IActionResult AssignMentor(Guid meetingId, string mentorId)
        {
            if (mentorId != null)
            {
                var result = _meetingService.AssignMentor(meetingId, mentorId);
                if (result)
                {
                    TempData["RequestSuccess"] = _localizer["Requests_SuccessfulRequestAssign"].ToString();

                    return RedirectToAction(nameof(MeetingRequests));
                }
                else
                {
                    TempData["RequestError"] = _localizer["Requests_UnsuccessfulRequestAssign"].ToString();
                }
            }

            TempData["RequestError"] = _localizer["Requests_PleaseChooseMentor"].ToString();

            return RedirectToAction(nameof(ShowMeetingRequestDetails), new { meetingId });
        }

        MeetingsGridViewModel BuildMeetingRequestModel(MeetingsFilterViewModel? filter = null)
        {
            var viewModel = new MeetingsGridViewModel
            {
                Filter = filter ?? new MeetingsFilterViewModel() { Page = 1, PageSize = PAGE_SIZE }
            };
            viewModel.Data = _meetingService.GetPendingMeetings(out var totalRecordsCount, viewModel.Filter.Page, viewModel.Filter.PageSize);

            viewModel.TotalRecordsCount = totalRecordsCount;

            return viewModel;
        }
    }
}
