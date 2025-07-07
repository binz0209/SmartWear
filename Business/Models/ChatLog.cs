using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Business.Models
{
    public class ChatLog : BaseEntity
    {
        [Required]
        public Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }

        [Required]
        public string UserQuestion { get; set; }

        [Required]
        public string BotResponse { get; set; }
    }
}
