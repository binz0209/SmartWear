using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartWear.ViewModels
{
    public class CartViewModel
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        public string UserFullName { get; set; } // Tên người dùng (optional, nếu cần)

        public List<CartItemViewModel> CartItems { get; set; }
    }
}
