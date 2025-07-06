using System;
using System.Collections.Generic;
using System.Linq;
using SmartWear.Models;

namespace SmartWear.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            // 1️⃣ Seed Roles
            if (!context.Roles.Any())
            {
                context.Roles.AddRange(
                    new Role { Id = Guid.NewGuid(), Name = "Admin", CreatedOn = DateTime.UtcNow, IsDeleted = false },
                    new Role { Id = Guid.NewGuid(), Name = "Customer", CreatedOn = DateTime.UtcNow, IsDeleted = false },
                    new Role { Id = Guid.NewGuid(), Name = "Staff", CreatedOn = DateTime.UtcNow, IsDeleted = false }
                );
                context.SaveChanges();
            }

            // 2️⃣ Seed Admin User
            var adminRole = context.Roles.FirstOrDefault(r => r.Name == "Admin");
            if (adminRole != null && !context.Users.Any(u => u.Username == "admin"))
            {
                var adminUser = new User
                {
                    Id = Guid.NewGuid(),
                    Username = "admin",
                    Email = "admin@smartwear.com",
                    PasswordHash = "admin123", // Plain text
                    RoleId = adminRole.Id,
                    CreatedOn = DateTime.UtcNow,
                    IsDeleted = false
                };

                context.Users.Add(adminUser);
                context.SaveChanges();
            }

            // 3️⃣ Seed Categories
            if (!context.Categories.Any())
            {
                var categories = new List<Category>
                {
                    new Category { Id = Guid.NewGuid(), Name = "Man", Description = "Quần áo nam", CreatedOn = DateTime.UtcNow, IsDeleted = false },
                    new Category { Id = Guid.NewGuid(), Name = "Women", Description = "Quần áo nữ", CreatedOn = DateTime.UtcNow, IsDeleted = false },
                    new Category { Id = Guid.NewGuid(), Name = "Kid", Description = "Quần áo trẻ em", CreatedOn = DateTime.UtcNow, IsDeleted = false }
                };

                context.Categories.AddRange(categories);
                context.SaveChanges();
            }

            // 4️⃣ Seed Products
            if (!context.Products.Any())
            {
                var manCategory = context.Categories.FirstOrDefault(c => c.Name == "Man");
                var womenCategory = context.Categories.FirstOrDefault(c => c.Name == "Women");

                if (manCategory != null && womenCategory != null)
                {
                    context.Products.AddRange(
                        new Product
                        {
                            Id = Guid.NewGuid(),
                            Name = "Classic White T-Shirt",
                            Description = "Soft cotton t-shirt for everyday wear.",
                            Price = 19.99m,
                            StockQuantity = 100,
                            CategoryId = manCategory.Id,
                            CreatedOn = DateTime.UtcNow,
                            IsDeleted = false
                        },
                        new Product
                        {
                            Id = Guid.NewGuid(),
                            Name = "Blue Denim Jeans",
                            Description = "Comfortable slim-fit jeans.",
                            Price = 49.99m,
                            StockQuantity = 50,
                            CategoryId = manCategory.Id,
                            CreatedOn = DateTime.UtcNow,
                            IsDeleted = false
                        },
                        new Product
                        {
                            Id = Guid.NewGuid(),
                            Name = "Red Summer Dress",
                            Description = "Light and breezy dress for summer days.",
                            Price = 59.99m,
                            StockQuantity = 30,
                            CategoryId = womenCategory.Id,
                            CreatedOn = DateTime.UtcNow,
                            IsDeleted = false
                        }
                    );
                    context.SaveChanges();
                }
            }
        }
    }
}
