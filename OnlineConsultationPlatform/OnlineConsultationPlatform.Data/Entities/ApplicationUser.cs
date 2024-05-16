using Microsoft.AspNetCore.Identity;

namespace OnlineConsultationPlatform.Data.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? ProfilePicturePath { get; set; }

        public bool IsActive { get; set; }
    }
}
