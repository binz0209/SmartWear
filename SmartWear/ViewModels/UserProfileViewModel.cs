using System.ComponentModel.DataAnnotations;

namespace SmartWear.ViewModels
{
    public class UserProfileViewModel
    {
        public Guid Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }
        public List<OrderViewModel> Orders { get; set; } = new List<OrderViewModel>();
    }


}
