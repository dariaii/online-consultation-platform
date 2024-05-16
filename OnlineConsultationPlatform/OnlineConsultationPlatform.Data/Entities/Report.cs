using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineConsultationPlatform.Data.Entities
{
    public class Report
    {
        [Key]
        public Guid Id { get; set; }

        public string? UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }

        public Guid? MeetingId { get; set; }

        [ForeignKey("MeetingId")]
        public virtual Meeting? Meeting { get; set; }

        public string? ReportFilePath { get; set; }

        public DateTime UploadDate { get; set; }
    }
}
