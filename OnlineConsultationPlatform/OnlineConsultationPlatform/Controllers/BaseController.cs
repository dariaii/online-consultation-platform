using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace OnlineConsultationPlatform.Controllers
{
    public class BaseController : Controller
    {
        [NonAction]
        protected void HandleErrors(IEnumerable<IdentityError> errorsList)
        {
            ViewBag.Errors = new List<string>();
            foreach (var error in errorsList)
            {
                ViewBag.Errors.Add(error.Description);
            }
        }

        [NonAction]
        protected void HandleErrors(string message)
        {
            ViewBag.Errors = new List<string>();
            ViewBag.Errors.Add(message);
        }
    }
}
