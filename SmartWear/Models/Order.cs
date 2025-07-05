using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartWear.Models
{
    public class Order : BaseEntity
    {
        [Required]
        public DateTime OrderDate { get; set; }

        [Required, MaxLength(50)]
        public string Status { get; set; }

        [Required]
        public Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }

        [Required]
        public Guid AddressId { get; set; }
        [ForeignKey("AddressId")]
        public Address Address { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }

        public Payment Payment { get; set; }
    }
}
