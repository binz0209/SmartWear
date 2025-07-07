using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Business.Models
{
    public class CartItem : BaseEntity
    {
        [Required]
        public Guid CartId { get; set; }
        [ForeignKey("CartId")]
        public Cart Cart { get; set; }

        [Required]
        public Guid ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        [Required]
        public int Quantity { get; set; }
    }
}
