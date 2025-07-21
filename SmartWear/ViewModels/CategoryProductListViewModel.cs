namespace SmartWear.ViewModels
{
    public class CategoryProductListViewModel
    {
        public string? SelectedCategory { get; set; }
        public List<ProductViewModel> Products { get; set; }
        // Thêm các thuộc tính phân trang
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 5; // số sản phẩm mỗi trang
    }
}
