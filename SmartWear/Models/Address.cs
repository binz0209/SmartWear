using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartWear.Models
{
    public class Address : BaseEntity
    {
        [Required]
        public Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }

        [Required, MaxLength(100)]
        public string FullName { get; set; }

        [Required, MaxLength(20)]
        public string PhoneNumber { get; set; }

        [Required, MaxLength(200)]
        public string StreetAddress { get; set; }

        [Required, MaxLength(100)]
        public string City { get; set; }
    }
}
