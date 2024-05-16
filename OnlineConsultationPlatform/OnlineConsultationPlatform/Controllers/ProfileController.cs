using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using OnlineConsultationPlatform.Core.Services;
using OnlineConsultationPlatform.ViewModels;

namespace OnlineConsultationPlatform.Controllers
{
    [Authorize]
    public class ProfileController(IProfileService profileService, IStringLocalizer localizer) : Controller
    {
        readonly IProfileService _profileService = profileService;
        readonly IStringLocalizer _localizer = localizer;

        [HttpGet]
        [Route("/profile")]
        public IActionResult Index()
        {
            var profile = _profileService.GetProfile();
            if (profile == null)
            {
                return NotFound();
            }

            var viewModel = new ProfileViewModel
            {
                FirstName = profile.FirstName,
                LastName = profile.LastName,
                PhoneNumber = profile.PhoneNumber,
                Email = profile.Email,
                ProfilePicturePath = profile.ProfilePicturePath
            };

            return View(viewModel);
        }

        [HttpPost]
        [Route("/profile")]
        public IActionResult Index(ProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                _profileService.EditProfile(
                    model.FirstName,
                    model.LastName,
                    model.PhoneNumber);

                return RedirectToAction("Index");
            }

            return View(model);
        }

        [HttpPost]
        [Route("/profile/update-avatar")]
        public IActionResult UpdateAvatar([FromForm] IFormFile file)
        {
            if (file != null && _profileService.UploadProfilePicture(file))
            {
                return RedirectToAction("Index");
            }

            TempData["Errors"] = _localizer["Global_ErrorMesage"].ToString();

            return RedirectToAction("Index");
        }
    }
}
