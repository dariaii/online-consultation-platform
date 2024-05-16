using System.ComponentModel.DataAnnotations;
using OnlineConsultationPlatform.Data.Entities;

namespace OnlineConsultationPlatform.ViewModels
{
    public class MeetingRequestViewModel
    {
        [Required(ErrorMessage = "Message_RequiredField")]
        public DateTime FirstDateOption { get; set; }

        [Required(ErrorMessage = "Message_RequiredField")]
        public DateTime SecondDateOption { get; set; }

        [Required(ErrorMessage = "Message_RequiredField")]
        public string? Subject { get; set; }

        [Required(ErrorMessage = "Message_RequiredField")]
        public string? FirstQuestion { get; set; }

        [Required(ErrorMessage = "Message_RequiredField")]
        public string? SecondQuestion { get; set; }

        [Required(ErrorMessage = "Message_RequiredField")]
        public string? ThirdQuestion { get; set; }

        public string? ExtraInformation { get; set; }

        public string? ChosenMentorId { get; set; }

        public List<ApplicationUser>? Mentors { get; set; }
    }
}
