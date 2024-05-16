namespace OnlineConsultationPlatform.ViewModels
{
    public class GridViewModel<T1, T2> where T2 : FilterViewModel
    {
        public List<T1> Data { get; set; }

        public int TotalRecordsCount { get; set; }

        public T2 Filter { get; set; }

        public int PagesCount
        {
            get
            {
                return (int)Math.Ceiling((double)TotalRecordsCount / Filter?.PageSize ?? 1d);
            }
        }

        public int? NextPage
        {
            get
            {
                return TotalRecordsCount > Filter?.Page * Filter?.PageSize ? Filter?.Page + 1 : (int?)null;
            }
        }

        public int? PreviousPage
        {
            get
            {
                return Filter?.Page < 2 ? (int?)null : Filter?.Page - 1;
            }
        }
    }
}
