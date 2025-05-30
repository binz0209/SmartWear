using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartWear.ViewModels
{
    public class RoleViewModel
    {
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Name { get; set; }

        public List<UserViewModel> Users { get; set; } = new List<UserViewModel>();
    }
}
