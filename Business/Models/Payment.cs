using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Business.Models
{
    public class Payment : BaseEntity
    {
        [Required]
        public Guid OrderId { get; set; }
        [ForeignKey("OrderId")]
        public Order Order { get; set; }

        [Required, MaxLength(50)]
        public string PaymentMethod { get; set; }

        [Required]
        public bool IsPaid { get; set; }

        public DateTime? PaymentDate { get; set; }
    }
}
