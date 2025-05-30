using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartWear.ViewModels
{
    public class ProductViewModel
    {
        public int Id { get; set; }

        [Required, MaxLength(150)]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive value.")]
        public decimal Price { get; set; }

        public string? ImageUrl { get; set; }

        [Required]
        public int StockQuantity { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public string CategoryName { get; set; }  // Optional: Category name to display

        public List<ProductReviewViewModel> ProductReviews { get; set; } = new List<ProductReviewViewModel>();
    }
}
