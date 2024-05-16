using Microsoft.AspNetCore.Mvc;
using OnlineConsultationPlatform.Core.Services;

namespace OnlineConsultationPlatform.ViewComponents
{
    public class ProfileViewComponent(IAuthenticationService authenticationService) : ViewComponent
    {
        private readonly IAuthenticationService _authenticationService = authenticationService;

        public IViewComponentResult Invoke()
        {
            var user = _authenticationService.CurrentUser();

            return View("../_ProfileRightSidebar", user);
        }
    }
}
