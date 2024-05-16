using System.ComponentModel.DataAnnotations;

namespace OnlineConsultationPlatform.ViewModels
{
    public class FeedbackViewModel
    {
        public Guid MeetingId { get; set; }

        [Required(ErrorMessage = "Message_RequiredField")]
        public int? Rating { get; set; }

        public string? FeedbackSubject { get; set; }

        public string? FeedbackText { get; set; }
    }
}
