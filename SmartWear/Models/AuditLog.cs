using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartWear.Models
{
    public class AuditLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [Required, MaxLength(100)]
        public string Action { get; set; }   // VD: CreateOrder, UpdateProduct, Login, etc.

        [MaxLength(500)]
        public string Description { get; set; }  // Chi tiết hành động

        [Required]
        public DateTime Timestamp { get; set; }

        [MaxLength(50)]
        public string IpAddress { get; set; }
    }
}
