using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartWear.ViewModels
{
    public class OrderViewModel
    {
        public int Id { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [Required, MaxLength(50)]
        public string Status { get; set; }

        [Required]
        public int UserId { get; set; }

        public string UserFullName { get; set; }  // Optional, nếu cần hiển thị tên người dùng

        [Required]
        public int AddressId { get; set; }

        public string AddressDetails { get; set; }  // Optional, nếu bạn muốn hiển thị thông tin địa chỉ

        public List<OrderItemViewModel> OrderItems { get; set; } = new List<OrderItemViewModel>();

        public PaymentViewModel Payment { get; set; }
    }
}
