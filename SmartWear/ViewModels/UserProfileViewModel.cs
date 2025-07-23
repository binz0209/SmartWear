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
        // Thêm cho đổi mật khẩu
        public string? CurrentPassword { get; set; }
        public string? NewPassword { get; set; }
        public string? ConfirmNewPassword { get; set; }

    }
}
