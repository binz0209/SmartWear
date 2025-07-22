namespace SmartWear.ViewModels
{
    public class SearchViewModel
    {
        public string Keyword { get; set; }
        public IEnumerable<ProductViewModel> Products { get; set; } = new List<ProductViewModel>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string SortOrder { get; set; }
    }
}
