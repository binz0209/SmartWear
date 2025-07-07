using Microsoft.EntityFrameworkCore;
using Business.Models;

namespace Business.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

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

            // ======== Role ========
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasIndex(r => r.Name).IsUnique();
                entity.Property(r => r.Name).HasMaxLength(50).IsRequired();
            });

            // ======== User ========
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Username).IsUnique();
                entity.HasIndex(u => u.Email).IsUnique();

                entity.Property(u => u.Username).HasMaxLength(100).IsRequired();
                entity.Property(u => u.Email).HasMaxLength(150).IsRequired();
                entity.Property(u => u.PasswordHash).IsRequired();

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

            // ======== Category ========
            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(c => c.Name).HasMaxLength(100).IsRequired();
                entity.HasMany(c => c.Products)
                      .WithOne(p => p.Category)
                      .HasForeignKey(p => p.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ======== Product ========
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

            // ======== Order ========
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

            // ======== OrderItem ========
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.Property(oi => oi.Quantity).IsRequired();
                entity.Property(oi => oi.UnitPrice).HasColumnType("decimal(18,2)").IsRequired();
            });

            // ======== Cart ========
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

            // ======== CartItem ========
            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.Property(ci => ci.Quantity).IsRequired();
            });

            // ======== ChatLog ========
            modelBuilder.Entity<ChatLog>(entity =>
            {
                entity.Property(cl => cl.UserQuestion).IsRequired();
                entity.Property(cl => cl.BotResponse).IsRequired();

                // BaseEntity đã có CreatedOn
            });

            // ======== Payment ========
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.Property(p => p.PaymentMethod).HasMaxLength(50).IsRequired();
                entity.Property(p => p.IsPaid).IsRequired();
            });

            // ======== ProductReview ========
            modelBuilder.Entity<ProductReview>(entity =>
            {
                entity.Property(pr => pr.Rating).IsRequired();
                entity.Property(pr => pr.Comment);
            });

            // ======== Address ========
            modelBuilder.Entity<Address>(entity =>
            {
                entity.Property(a => a.FullName).HasMaxLength(100).IsRequired();
                entity.Property(a => a.PhoneNumber).HasMaxLength(20).IsRequired();
                entity.Property(a => a.StreetAddress).HasMaxLength(200).IsRequired();
                entity.Property(a => a.City).HasMaxLength(100).IsRequired();
            });

            // ======== AuditLog ========
            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.Property(al => al.Action).HasMaxLength(100).IsRequired();
                entity.Property(al => al.Description).HasMaxLength(500);
                entity.Property(al => al.IpAddress).HasMaxLength(50);
                // BaseEntity đã có CreatedOn
            });
        }
    }
}
