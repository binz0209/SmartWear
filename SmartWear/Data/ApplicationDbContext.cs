using Microsoft.EntityFrameworkCore;
using SmartWear.Models;

namespace SmartWear.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<ChatLog> ChatLogs { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<ProductReview> ProductReviews { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // AuditLog
            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.Property(al => al.Action)
                      .HasMaxLength(100)
                      .IsRequired();

                entity.Property(al => al.Description)
                      .HasMaxLength(500);

                entity.Property(al => al.Timestamp)
                      .IsRequired();

                entity.Property(al => al.IpAddress)
                      .HasMaxLength(50);

                entity.HasOne(al => al.User)
                      .WithMany()
                      .HasForeignKey(al => al.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Role
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasIndex(r => r.Name).IsUnique();
                entity.Property(r => r.Name).HasMaxLength(50).IsRequired();
            });

            // User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Username).IsUnique();
                entity.HasIndex(u => u.Email).IsUnique();

                entity.Property(u => u.Username).HasMaxLength(100).IsRequired();
                entity.Property(u => u.Email).HasMaxLength(150).IsRequired();
                entity.Property(u => u.Password).IsRequired();

                entity.HasOne(u => u.Role)
                      .WithMany(r => r.Users)
                      .HasForeignKey(u => u.RoleId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(u => u.Cart)
                      .WithOne(c => c.User)
                      .HasForeignKey<Cart>(c => c.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(u => u.Addresses)
                      .WithOne(a => a.User)
                      .HasForeignKey(a => a.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(u => u.Orders)
                      .WithOne(o => o.User)
                      .HasForeignKey(o => o.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(u => u.ProductReviews)
                      .WithOne(r => r.User)
                      .HasForeignKey(r => r.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(u => u.ChatLogs)
                      .WithOne(cl => cl.User)
                      .HasForeignKey(cl => cl.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Category
            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(c => c.Name).HasMaxLength(100).IsRequired();
                entity.HasMany(c => c.Products)
                      .WithOne(p => p.Category)
                      .HasForeignKey(p => p.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Product
            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(p => p.Name).HasMaxLength(150).IsRequired();
                entity.Property(p => p.Price).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(p => p.StockQuantity).IsRequired();

                entity.HasMany(p => p.OrderItems)
                      .WithOne(oi => oi.Product)
                      .HasForeignKey(oi => oi.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(p => p.CartItems)
                      .WithOne(ci => ci.Product)
                      .HasForeignKey(ci => ci.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(p => p.ProductReviews)
                      .WithOne(pr => pr.Product)
                      .HasForeignKey(pr => pr.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Order
            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(o => o.OrderDate).IsRequired();
                entity.Property(o => o.Status).HasMaxLength(50).IsRequired();

                entity.HasOne(o => o.User)
                      .WithMany(u => u.Orders)
                      .HasForeignKey(o => o.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(o => o.Address)
                      .WithMany()
                      .HasForeignKey(o => o.AddressId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(o => o.OrderItems)
                      .WithOne(oi => oi.Order)
                      .HasForeignKey(oi => oi.OrderId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(o => o.Payment)
                      .WithOne(p => p.Order)
                      .HasForeignKey<Payment>(p => p.OrderId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // OrderItem
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.Property(oi => oi.Quantity).IsRequired();
                entity.Property(oi => oi.UnitPrice).HasColumnType("decimal(18,2)").IsRequired();

                entity.HasOne(oi => oi.Order)
                      .WithMany(o => o.OrderItems)
                      .HasForeignKey(oi => oi.OrderId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(oi => oi.Product)
                      .WithMany(p => p.OrderItems)
                      .HasForeignKey(oi => oi.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Cart
            modelBuilder.Entity<Cart>(entity =>
            {
                entity.HasOne(c => c.User)
                      .WithOne(u => u.Cart)
                      .HasForeignKey<Cart>(c => c.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(c => c.CartItems)
                      .WithOne(ci => ci.Cart)
                      .HasForeignKey(ci => ci.CartId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // CartItem
            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.HasKey(ci => ci.Id);

                entity.Property(ci => ci.Quantity).IsRequired();

                entity.HasOne(ci => ci.Cart)
                      .WithMany(c => c.CartItems)
                      .HasForeignKey(ci => ci.CartId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ci => ci.Product)
                      .WithMany(p => p.CartItems)
                      .HasForeignKey(ci => ci.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ChatLog
            modelBuilder.Entity<ChatLog>(entity =>
            {
                entity.Property(cl => cl.UserQuestion).IsRequired();
                entity.Property(cl => cl.BotResponse).IsRequired();
                entity.Property(cl => cl.CreatedAt).HasDefaultValueSql("GETDATE()").IsRequired();

                entity.HasOne(cl => cl.User)
                      .WithMany(u => u.ChatLogs)
                      .HasForeignKey(cl => cl.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Payment
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.Property(p => p.PaymentMethod).HasMaxLength(50).IsRequired();
                entity.Property(p => p.IsPaid).IsRequired();

                entity.HasOne(p => p.Order)
                      .WithOne(o => o.Payment)
                      .HasForeignKey<Payment>(p => p.OrderId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ProductReview
            modelBuilder.Entity<ProductReview>(entity =>
            {
                entity.Property(pr => pr.Rating).IsRequired();
                entity.Property(pr => pr.Comment);
                entity.Property(pr => pr.CreatedAt).HasDefaultValueSql("GETDATE()").IsRequired();

                entity.HasOne(pr => pr.Product)
                      .WithMany(p => p.ProductReviews)
                      .HasForeignKey(pr => pr.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(pr => pr.User)
                      .WithMany(u => u.ProductReviews)
                      .HasForeignKey(pr => pr.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Address
            modelBuilder.Entity<Address>(entity =>
            {
                entity.Property(a => a.FullName).HasMaxLength(100).IsRequired();
                entity.Property(a => a.PhoneNumber).HasMaxLength(20).IsRequired();
                entity.Property(a => a.StreetAddress).HasMaxLength(200).IsRequired();
                entity.Property(a => a.City).HasMaxLength(100).IsRequired();

                entity.HasOne(a => a.User)
                      .WithMany(u => u.Addresses)
                      .HasForeignKey(a => a.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
