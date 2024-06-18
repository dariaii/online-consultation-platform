using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineConsultationPlatform.Core.Services;
using OnlineConsultationPlatform.Core.Infrastructure.Enums;

namespace OnlineConsultationPlatform.Controllers
{
    [Authorize(Roles = nameof(Roles.Mentor))]
    public class MentorsController(IUsersService usersService) : Controller
    {
        private readonly IUsersService _usersService = usersService;

        [HttpGet]
        [Route("/mentors")]
        public IActionResult ActiveMentors()
        {
            return View(_usersService.GetMentors());
        }
    }
}
