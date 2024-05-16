using OnlineConsultationPlatform.Data.Entities;

namespace OnlineConsultationPlatform.ViewModels
{
    public class MeetingsGridViewModel : GridViewModel<Meeting, MeetingsFilterViewModel>
    {
    }

    public class MeetingsFilterViewModel : FilterViewModel
    {
        public bool ShowUnconfirmed { get; set; }
    }
}
