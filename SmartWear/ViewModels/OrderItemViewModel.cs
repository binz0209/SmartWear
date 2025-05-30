using System.ComponentModel.DataAnnotations;

namespace SmartWear.ViewModels
{
    public class OrderItemViewModel
    {
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        public string ProductName { get; set; } // Tên sản phẩm (optional, nếu bạn muốn hiển thị)

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal UnitPrice { get; set; }

        public decimal TotalPrice => Quantity * UnitPrice; // Tính tổng tiền cho mặt hàng
    }
}
