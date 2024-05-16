using OnlineConsultationPlatform.Data.Entities;

namespace OnlineConsultationPlatform.ViewModels
{
    public class UserStatisticsViewModel
    {
        public ApplicationUser? User { get; set; }

        public int NumberOfMeetingsBooked { get; set; }

        public int NumberOfRatings { get; set; }

        public int AverageRating { get; set; }
    }
}
