namespace OnlineConsultationPlatform.ViewModels
{
    public class FilterViewModel
    {
        public int? Page { get; set; }

        public int? PageSize { get; set; }

        public bool HasSelectedFilter<T>(params List<FilterItemViewModel<T>>[] filters)
        {
            return filters?.Length > 0 && filters.Any(filter => filter.Any(record => record.IsSelected));
        }
    }

    public class FilterItemViewModel<T>
    {
        public T Value { get; set; }

        public bool IsSelected { get; set; }
    }
}
