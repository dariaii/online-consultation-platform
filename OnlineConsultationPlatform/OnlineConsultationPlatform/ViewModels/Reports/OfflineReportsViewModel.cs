using System.ComponentModel.DataAnnotations;

namespace OnlineConsultationPlatform.ViewModels
{
    public class OfflineReportsViewModel
    {
        [Required(ErrorMessage = "Message_RequiredField")]
        public DateTime? MeetingDate { get; set; }

        [Required(ErrorMessage = "Message_RequiredField")]
        public string? StudentName { get; set; }

        [Required(ErrorMessage = "Message_RequiredField")]
        public IFormFile? ReportFile { get; set; }
    }
}
