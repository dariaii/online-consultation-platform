using System.ComponentModel.DataAnnotations;

namespace OnlineConsultationPlatform.ViewModels
{
    public class ProfileViewModel
    {
        [Required(ErrorMessage = "Message_RequiredField")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Message_RequiredField")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Message_RequiredField")]
        public string? PhoneNumber { get; set; }

        public string? Email { get; set; }

        public string? ProfilePicturePath { get; set; }

        public IFormFile? ProfilePictureFile { get; set; }
    }
}
