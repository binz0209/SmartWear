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

            // 2️⃣ Seed Admin User
            var adminRole = context.Roles.FirstOrDefault(r => r.Name == "Admin");
            if (adminRole != null && !context.Users.Any(u => u.Username == "admin"))
            {
                var adminUser = new User
                {
                    Id = Guid.NewGuid(),
                    Username = "admin",
                    Email = "admin@smartwear.com",
                    PasswordHash = "admin123", // Plain text, you should hash it for real use
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

            // 5️⃣ Seed Orders
            if (!context.Orders.Any())
            {
                var adminUserId = context.Users.FirstOrDefault(u => u.Username == "admin")?.Id;
                var addressId = context.Addresses.FirstOrDefault(a => a.UserId == adminUserId)?.Id;

                if (adminUserId != null && addressId != null)
                {
                    var order = new Order
                    {
                        Id = Guid.NewGuid(),
                        OrderDate = DateTime.UtcNow,
                        Status = "Pending",
                        UserId = adminUserId.Value,
                        AddressId = addressId.Value,
                        CreatedOn = DateTime.UtcNow,
                        IsDeleted = false
                    };
                    context.Orders.Add(order);
                    context.SaveChanges();
                }
            }

            // 6️⃣ Seed OrderItems
            if (!context.OrderItems.Any())
            {
                var orderId = context.Orders.FirstOrDefault()?.Id;
                var productId = context.Products.FirstOrDefault()?.Id;

                if (orderId != null && productId != null)
                {
                    var orderItem = new OrderItem
                    {
                        Id = Guid.NewGuid(),
                        Quantity = 2,
                        UnitPrice = 19.99m,
                        OrderId = orderId.Value,
                        ProductId = productId.Value,
                        CreatedOn = DateTime.UtcNow,
                        IsDeleted = false
                    };
                    context.OrderItems.Add(orderItem);
                    context.SaveChanges();
                }
            }

            // 7️⃣ Seed Payments
            if (!context.Payments.Any())
            {
                var orderId = context.Orders.FirstOrDefault()?.Id;

                if (orderId != null)
                {
                    var payment = new Payment
                    {
                        Id = Guid.NewGuid(),
                        OrderId = orderId.Value,
                        PaymentMethod = "Credit Card",
                        IsPaid = true,
                        PaymentDate = DateTime.UtcNow,
                        CreatedOn = DateTime.UtcNow,
                        IsDeleted = false
                    };
                    context.Payments.Add(payment);
                    context.SaveChanges();
                }
            }

            // 8️⃣ Seed CartItems
            if (!context.CartItems.Any())
            {
                var cartId = context.Carts.FirstOrDefault()?.Id;
                var productId = context.Products.FirstOrDefault()?.Id;

                if (cartId != null && productId != null)
                {
                    var cartItem = new CartItem
                    {
                        Id = Guid.NewGuid(),
                        CartId = cartId.Value,
                        ProductId = productId.Value,
                        Quantity = 1,
                        CreatedOn = DateTime.UtcNow,
                        IsDeleted = false
                    };
                    context.CartItems.Add(cartItem);
                    context.SaveChanges();
                }
            }

            // 9️⃣ Seed ChatLogs
            if (!context.ChatLogs.Any())
            {
                var userId = context.Users.FirstOrDefault()?.Id;

                if (userId != null)
                {
                    var chatLog = new ChatLog
                    {
                        Id = Guid.NewGuid(),
                        UserId = userId.Value,
                        UserQuestion = "What is my order status?",
                        BotResponse = "Your order is pending.",
                        CreatedOn = DateTime.UtcNow,
                        IsDeleted = false
                    };
                    context.ChatLogs.Add(chatLog);
                    context.SaveChanges();
                }
            }

            // 🔟 Seed ProductReviews
            if (!context.ProductReviews.Any())
            {
                var productId = context.Products.FirstOrDefault()?.Id;
                var userId = context.Users.FirstOrDefault()?.Id;

                if (productId != null && userId != null)
                {
                    var productReview = new ProductReview
                    {
                        Id = Guid.NewGuid(),
                        ProductId = productId.Value,
                        UserId = userId.Value,
                        Rating = 5,
                        Comment = "Great product!",
                        CreatedOn = DateTime.UtcNow,
                        IsDeleted = false
                    };
                    context.ProductReviews.Add(productReview);
                    context.SaveChanges();
                }
            }
        }
    }
}
