using System.ComponentModel.DataAnnotations;

namespace SmartWear.ViewModels
{
    public class CartItemViewModel
    {
        public int Id { get; set; }

        [Required]
        public int CartId { get; set; }

        public string ProductName { get; set; } // Tên sản phẩm, có thể hiển thị trong giao diện

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; } // Giá đơn vị sản phẩm

        public decimal TotalPrice => Quantity * UnitPrice; // Tổng giá trị của sản phẩm trong giỏ hàng
    }
}
