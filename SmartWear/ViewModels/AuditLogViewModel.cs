using System;
using System.ComponentModel.DataAnnotations;

namespace SmartWear.ViewModels
{
    public class AuditLogViewModel
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required, MaxLength(100)]
        public string Action { get; set; }  // VD: CreateOrder, UpdateProduct, Login, etc.

        [MaxLength(500)]
        public string Description { get; set; }  // Chi tiết hành động

        [Required]
        public DateTime Timestamp { get; set; }

        [MaxLength(50)]
        public string IpAddress { get; set; }
    }
}
