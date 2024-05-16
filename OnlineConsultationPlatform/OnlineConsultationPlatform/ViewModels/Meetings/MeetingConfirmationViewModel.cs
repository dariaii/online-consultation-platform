using System.ComponentModel.DataAnnotations;

namespace OnlineConsultationPlatform.ViewModels
{
    public class MeetingConfirmationViewModel
    {
        [Required(ErrorMessage = "Message_RequiredField")]
        public Guid? MeetingId { get; set; }

        [Required(ErrorMessage = "Message_RequiredField")]
        public DateTime? Date { get; set; }
    }
}
