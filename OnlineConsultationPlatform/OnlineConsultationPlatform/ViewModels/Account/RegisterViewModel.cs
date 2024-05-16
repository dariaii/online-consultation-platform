using System.ComponentModel.DataAnnotations;

namespace OnlineConsultationPlatform.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Message_RequiredField")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Message_RequiredField")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Message_RequiredField")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Message_RequiredField")]
        [EmailAddress(ErrorMessage = "Message_InvalidEmail")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Message_RequiredField")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Message_RequiredField")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Message_PasswordsDontMatch")]
        public string? ConfirmPassword { get; set; }

        public bool IsMentor { get; set; }

        [Range(typeof(bool), "true", "true", ErrorMessage = "Message_RequiredField")]
        public bool HasAgreedWithTermsAndConditions { get; set; }

        public string? ReturnUrl { get; set; }
    }
}
