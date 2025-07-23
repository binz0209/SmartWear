namespace SmartWear.ViewModels
{
    public class OrderViewModel
    {
        public Guid Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public string StatusDisplay { get; set; }

        // Tiền
        public decimal Subtotal { get; set; }
        public decimal Shipping { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }

        // Thanh toán
        public string PaymentMethodDisplay { get; set; }

        // Danh sách ảnh thumbnail
        public IList<string> ProductThumbnails { get; set; }

        // Chi tiết từng item
        public IList<OrderItemViewModel> Items { get; set; }

        // Giao hàng
        public string ShippingAddress { get; set; }
        public string ShippingMethod { get; set; }
    }
}
