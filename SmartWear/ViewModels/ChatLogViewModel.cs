using System;
using System.ComponentModel.DataAnnotations;

namespace SmartWear.ViewModels
{
    public class ChatLogViewModel
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        public string UserFullName { get; set; }  // Tên người dùng (optional)

        [Required]
        public string UserQuestion { get; set; }

        [Required]
        public string BotResponse { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }
    }
}
