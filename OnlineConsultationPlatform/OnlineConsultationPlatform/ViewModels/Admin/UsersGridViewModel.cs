namespace OnlineConsultationPlatform.ViewModels
{
    public class UsersGridViewModel : GridViewModel<UserStatisticsViewModel, UsersFilterViewModel>
    {
    }

    public class UsersFilterViewModel : FilterViewModel
    {
        public bool ShowDeactivated { get; set; }
    }
}
