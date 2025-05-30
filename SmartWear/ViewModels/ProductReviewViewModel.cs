using System.ComponentModel.DataAnnotations;

namespace SmartWear.ViewModels
{
    public class ProductReviewViewModel
    {
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        public string ProductName { get; set; }  // Optional: Product name to display with review

        [Required]
        public int UserId { get; set; }

        public string UserFullName { get; set; } // Optional: User's full name

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }

        public string Comment { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
