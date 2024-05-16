using OnlineConsultationPlatform.Data.Entities;

namespace OnlineConsultationPlatform.ViewModels
{
    public class ReportsGridViewModel : GridViewModel<Report, ReportsFilterViewModel>
    {
        public List<Meeting>? Meetings { get; set; }
    }

    public class ReportsFilterViewModel : FilterViewModel 
    { 
    }
}
