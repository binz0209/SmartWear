using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartWear.Models
{
    public class ChatLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [Required]
        public string UserQuestion { get; set; }

        [Required]
        public string BotResponse { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }
    }
}
