using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Business.Models
{
    public class Role : BaseEntity
    {
        [Required, MaxLength(50)]
        public string Name { get; set; }

        public ICollection<User> Users { get; set; }
    }
}
