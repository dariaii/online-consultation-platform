using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineConsultationPlatform.Data.Entities
{
    public class Meeting
    {
        [Key]
        public Guid Id { get; set; }

        public string? UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }

        public string? MentorId { get; set; }

        [ForeignKey("MentorId")]
        public virtual ApplicationUser? Mentor { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? MeetingDate { get; set; }

        public string? MeetingUrl { get; set; }

        public int? Rating { get; set; }

        public DateTime FirstDateOption { get; set; }

        public DateTime SecondDateOption { get; set; }

        public string? Subject { get; set; }

        public string? FirstQuestion { get; set; }

        public string? SecondQuestion { get; set; }

        public string? ThirdQuestion { get; set; }

        public string? ExtraInformation { get; set; }

        public bool IsDeclined { get; set; }

        public virtual ICollection<Report> Reports { get; set; } = new HashSet<Report>();
    }
}
