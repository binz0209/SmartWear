using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartWear.ViewModels
{
    public class UserViewModel
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Username { get; set; }

        [Required, MaxLength(150)]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public int RoleId { get; set; }

        public string RoleName { get; set; }  // Optional: The role name for display in UI

        public List<AddressViewModel> Addresses { get; set; } = new List<AddressViewModel>();

        public List<OrderViewModel> Orders { get; set; } = new List<OrderViewModel>();

        public List<ProductReviewViewModel> ProductReviews { get; set; } = new List<ProductReviewViewModel>();

        public CartViewModel Cart { get; set; }

        public List<ChatLogViewModel> ChatLogs { get; set; } = new List<ChatLogViewModel>();
    }
}

