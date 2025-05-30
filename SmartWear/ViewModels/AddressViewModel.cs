using System.ComponentModel.DataAnnotations;

namespace SmartWear.ViewModels
{
    public class AddressViewModel
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

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
