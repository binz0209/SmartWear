using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartWear.Data;
using SmartWear.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartWear.ViewModels
{
    public class CategoryViewModel
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        public string? Description { get; set; }

        public List<ProductViewModel> Products { get; set; } = new List<ProductViewModel>();
    }
}

