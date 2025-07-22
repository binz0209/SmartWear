namespace SmartWear.ViewModels
{
    public class HomeViewModel
    {
        public List<ProductViewModel> NewArrivals { get; set; }
        public List<ProductViewModel> SaleProducts { get; set; }
        public List<ProductViewModel> FeaturedProducts { get; set; }
        public List<ProductViewModel> BestSellers { get; set; }
        public List<ProductViewModel> AllProducts { get; set; }
    }
}
