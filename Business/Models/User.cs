using Business.Models.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Business.Models
{
    public class User : BaseEntity
    {
        [Required, MaxLength(100)]
        public string Username { get; set; }

        [Required, MaxLength(150)]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        public Guid RoleId { get; set; }
        [ForeignKey("RoleId")]
        public Role Role { get; set; }

        public ICollection<Address> Addresses { get; set; }
        public ICollection<Order> Orders { get; set; }
        public Cart Cart { get; set; }
        public ICollection<ChatLog> ChatLogs { get; set; }
        public ICollection<ProductReview> ProductReviews { get; set; }
    }
}
