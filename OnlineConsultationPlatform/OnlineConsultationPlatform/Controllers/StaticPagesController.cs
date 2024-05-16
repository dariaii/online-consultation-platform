using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OnlineConsultationPlatform.Controllers
{
    public class StaticPagesController : Controller
    {
        [HttpGet]
        [Authorize]
        [Route("/help")]
        public IActionResult Help()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("/terms-and-conditions")]
        public IActionResult TermsAndConditions()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("/sitemap.xml")]
        public IActionResult Sitemap()
        {
            return File("sitemap.xml", "text/xml");
        }
    }
}
