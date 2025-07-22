using System;
using System.Collections.Generic;
using System.Linq;
using Business.Models;

namespace Business.Data
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


            var manCategory = context.Categories.FirstOrDefault(c => c.Name == "Man");
            var womenCategory = context.Categories.FirstOrDefault(c => c.Name == "Women");

            if (manCategory != null && womenCategory != null)
            {
                if (!context.Products.Any(p => p.Name == "Classic White T-Shirt"))
                {
                    context.Products.Add(new Product
                    {
                        Id = Guid.NewGuid(),
                        Name = "Classic White T-Shirt",
                        Description = "Soft cotton t-shirt for everyday wear.",
                        Price = 19.99m,
                        StockQuantity = 100,
                        CategoryId = manCategory.Id,
                        CreatedOn = DateTime.UtcNow,
                        IsDeleted = false
                    });
                }

                if (!context.Products.Any(p => p.Name == "Blue Denim Jeans"))
                {
                    context.Products.Add(new Product
                    {
                        Id = Guid.NewGuid(),
                        Name = "Blue Denim Jeans",
                        Description = "Comfortable slim-fit jeans.",
                        Price = 49.99m,
                        StockQuantity = 50,
                        CategoryId = manCategory.Id,
                        CreatedOn = DateTime.UtcNow,
                        IsDeleted = false
                    });
                }

                if (!context.Products.Any(p => p.Name == "Red Summer Dress"))
                {
                    context.Products.Add(new Product
                    {
                        Id = Guid.NewGuid(),
                        Name = "Red Summer Dress",
                        Description = "Light and breezy dress for summer days.",
                        Price = 59.99m,
                        StockQuantity = 30,
                        CategoryId = womenCategory.Id,
                        CreatedOn = DateTime.UtcNow,
                        IsDeleted = false
                    });
                }

                if (!context.Products.Any(p => p.Name == "Black Leather Jacket"))
                {
                    context.Products.Add(new Product
                    {
                        Id = Guid.NewGuid(),
                        Name = "Black Leather Jacket",
                        Description = "Stylish black leather for cool weather.",
                        Price = 99.99m,
                        Color = "black",
                        ImageUrl = "/img/product/black_leather_jacket.jpg",
                        StockQuantity = 20,
                        CategoryId = manCategory.Id,
                        CreatedOn = DateTime.UtcNow,
                        IsDeleted = false,
                        Size = "M"
                    });
                }

                if (!context.Products.Any(p => p.Name == "Green Hoodie"))
                {
                    context.Products.Add(new Product
                    {
                        Id = Guid.NewGuid(),
                        Name = "Green Hoodie",
                        Description = "Comfortable green hoodie for casual wear.",
                        Price = 39.99m,
                        Color = "green",
                        ImageUrl = "/img/product/green_hoodie.jpg",
                        StockQuantity = 25,
                        CategoryId = manCategory.Id,
                        CreatedOn = DateTime.UtcNow,
                        IsDeleted = false,
                        Size = "M"
                    });
                }

                if (!context.Products.Any(p => p.Name == "Pink Floral Blouse"))
                {
                    context.Products.Add(new Product
                    {
                        Id = Guid.NewGuid(),
                        Name = "Pink Floral Blouse",
                        Description = "Elegant blouse with pink floral design.",
                        Price = 34.99m,
                        Color = "pink",
                        ImageUrl = "/img/product/pink_floral_blouse.jpg",
                        StockQuantity = 40,
                        CategoryId = womenCategory.Id,
                        CreatedOn = DateTime.UtcNow,
                        IsDeleted = false,
                        Size = "L"
                    });
                }

                if (!context.Products.Any(p => p.Name == "Yellow Raincoat"))
                {
                    context.Products.Add(new Product
                    {
                        Id = Guid.NewGuid(),
                        Name = "Yellow Raincoat",
                        Description = "Bright yellow raincoat for stormy days.",
                        Price = 69.99m,
                        Color = "yellow",
                        ImageUrl = "/img/product/yellow_raincoat.jpg",
                        StockQuantity = 15,
                        CategoryId = womenCategory.Id,
                        CreatedOn = DateTime.UtcNow,
                        IsDeleted = false,
                        Size = "S"
                    });
                }

                if (!context.Products.Any(p => p.Name == "Purple Evening Gown"))
                {
                    context.Products.Add(new Product
                    {
                        Id = Guid.NewGuid(),
                        Name = "Purple Evening Gown",
                        Description = "Elegant purple gown for special occasions.",
                        Price = 149.99m,
                        Color = "purple",
                        ImageUrl = "/img/product/purple_evening_gown.jpg",
                        StockQuantity = 10,
                        CategoryId = womenCategory.Id,
                        CreatedOn = DateTime.UtcNow,
                        IsDeleted = false,
                        Size = "XL"
                    });
                }

                context.SaveChanges();
            }
        }
    }
}
