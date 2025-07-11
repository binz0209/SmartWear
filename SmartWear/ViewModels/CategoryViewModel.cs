namespace SmartWear.ViewModels
{
    public class CategoryViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<ProductViewModel> Products { get; set; } // Danh sách sản phẩm của danh mục này
    }
}
