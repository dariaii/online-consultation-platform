using System.ComponentModel.DataAnnotations;

namespace OnlineConsultationPlatform.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Message_RequiredField")]
        [EmailAddress(ErrorMessage = "Message_InvalidEmail")]
        public string? Email { get; set; }

        public string? ReturnUrl { get; set; }
    }
}
