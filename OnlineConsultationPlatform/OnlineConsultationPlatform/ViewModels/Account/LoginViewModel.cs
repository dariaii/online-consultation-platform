using System.ComponentModel.DataAnnotations;

namespace OnlineConsultationPlatform.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Message_RequiredField")]
        [EmailAddress(ErrorMessage = "Message_InvalidEmail")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Message_RequiredField")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        public bool RememberMe { get; set; }

        public string? ReturnUrl { get; set; }
    }
}
