using System.ComponentModel.DataAnnotations;

namespace SmartWear.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        public string? Description { get; set; }

        public ICollection<Product> Products { get; set; }
    }
}
