using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using OnlineConsultationPlatform.Core.Services;
using OnlineConsultationPlatform.ViewModels;
using OnlineConsultationPlatform.Core.Infrastructure.Enums;

namespace OnlineConsultationPlatform.Controllers
{
    [Authorize(Roles = nameof(Roles.Admin) + "," + nameof(Roles.Mentor))]
    public class ReportsController(
        IReportsService reportsService,
        IMeetingService meetingService,
        IConfiguration configuration,
        IStringLocalizer localizer,
        IFileService fileService) : Controller
    {
        readonly IReportsService _reportsService = reportsService;
        readonly IMeetingService _meetingService = meetingService;
        readonly IStringLocalizer _localizer = localizer;
        readonly IFileService _fileService = fileService;

        readonly int PAGE_SIZE = configuration.GetValue<int>("PaginationSettings:ReportsPageSize");

        [HttpGet]
        [Route("/reports")]
        public IActionResult Index(int? page = null, int? pageSize = null)
        {
            var viewModel = new ReportsGridViewModel
            {
                Filter = new ReportsFilterViewModel() { Page = page ?? 1, PageSize = pageSize ?? PAGE_SIZE },
                Meetings = _meetingService.GetMeetingsWithoutReports()
            };

            viewModel.Data = _reportsService.GetReports(out var totalRecordsCount, viewModel.Filter.Page, viewModel.Filter.PageSize);
            viewModel.TotalRecordsCount = totalRecordsCount;

            return View("Reports", viewModel);
        }

        [HttpPost]
        [Route("/reports/new")]
        public IActionResult UploadReport([FromForm] IFormFile? file, [FromForm] Guid? meetingId)
        {
            bool result = false;

            if (file != null)
            {
                result = _reportsService.SaveReport(file, meetingId);
            }

            if (result)
            {
                TempData["UploadReportSuccess"] = _localizer["Reports_SuccessMessage"].ToString();
            }
            else
            {
                TempData["UploadReportError"] = _localizer["Reports_ErrorMessage"].ToString();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Route("/reports/download")]
        public IActionResult DownloadReport(string fileName)
        {
            var file = _fileService.GetReportFile(fileName);

            if (file == null)
            {
                return NotFound();
            }
                
            return File(file, "application/octet-stream", fileName);
        }

        [HttpGet]
        [Route("/reports/offline/new")]
        public IActionResult UploadOfflineReport()
        {
            return View(new OfflineReportsViewModel());
        }

        [HttpPost]
        [Route("/reports/offline/new")]
        public IActionResult UploadOfflineReport(OfflineReportsViewModel model)
        {
            var result = _reportsService.SaveReport(model.ReportFile, null, model.MeetingDate, model.StudentName);

            if (result)
            {
                TempData["UploadReportSuccess"] = _localizer["Reports_SuccessMessage"].ToString();
            }
            else
            {
                TempData["UploadReportError"] = _localizer["Reports_ErrorMessage"].ToString();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
