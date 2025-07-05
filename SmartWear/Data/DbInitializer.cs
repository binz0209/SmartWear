using System.Collections.Generic;
using System.Linq;
using SmartWear.Models;

namespace SmartWear.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // Tạo database nếu chưa tồn tại
            context.Database.EnsureCreated();

            // Seed Roles
            if (!context.Roles.Any())
            {
                context.Roles.AddRange(
                    new Role { Name = "Admin" },
                    new Role { Name = "Customer" },
                    new Role { Name = "Staff" }
                );
                context.SaveChanges();
            }

            // Seed Admin User
            var adminRole = context.Roles.FirstOrDefault(r => r.Name == "Admin");

            if (adminRole != null && !context.Users.Any(u => u.RoleId == adminRole.Id))
            {
                var adminUser = new User
                {
                    Username = "admin",
                    Email = "admin@smartwear.com",
                    RoleId = adminRole.Id
                };

                // Bắt buộc: Hash mật khẩu
                var passwordHasher = new Microsoft.AspNetCore.Identity.PasswordHasher<User>();
                adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, "admin123");

                context.Users.Add(adminUser);
                context.SaveChanges();
            }


            // Seed Categories
            if (!context.Categories.Any())
            {
                var categories = new List<Category>
                {
                    new Category { Name = "Man", Description = "Danh mục quần áo nam" },
                    new Category { Name = "Women", Description = "Danh mục quần áo nữ" },
                    new Category { Name = "Kid", Description = "Danh mục quần áo trẻ em" }
                };

                context.Categories.AddRange(categories);
                context.SaveChanges();
            }

            // Seed Products
            if (!context.Products.Any())
            {
                var manCategory = context.Categories.First(c => c.Name == "Man");
                var womenCategory = context.Categories.First(c => c.Name == "Women");

                context.Products.AddRange(
                    new Product
                    {
                        Name = "Classic White T-Shirt",
                        Description = "Soft cotton t-shirt for everyday wear.",
                        Price = 19.99m,
                        StockQuantity = 100,
                        CategoryId = manCategory.Id
                    },
                    new Product
                    {
                        Name = "Blue Denim Jeans",
                        Description = "Comfortable slim-fit jeans.",
                        Price = 49.99m,
                        StockQuantity = 50,
                        CategoryId = manCategory.Id
                    },
                    new Product
                    {
                        Name = "Red Summer Dress",
                        Description = "Light and breezy dress for summer days.",
                        Price = 59.99m,
                        StockQuantity = 30,
                        CategoryId = womenCategory.Id
                    }
                );
                context.SaveChanges();
            }
        }
    }
}
