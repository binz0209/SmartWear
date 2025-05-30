using System;
using System.ComponentModel.DataAnnotations;

namespace SmartWear.ViewModels
{
    public class PaymentViewModel
    {
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }

        public string OrderStatus { get; set; } // Trạng thái của đơn hàng (optional)

        [Required]
        public string PaymentMethod { get; set; }

        [Required]
        public bool IsPaid { get; set; }

        public DateTime? PaymentDate { get; set; }
    }
}
