using Microsoft.EntityFrameworkCore;
using Business.Data;
using Repository;
using Services;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Services.VnPay;
using Repositories.Interfaces;
using Services.Interfaces;

namespace SmartWear
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Add ApplicationDbContext to DI container
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Add repositories to DI container
            builder.Services.AddScoped<IAddressRepository, AddressRepository>();
            builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
            builder.Services.AddScoped<ICartItemRepository, CartItemRepository>();
            builder.Services.AddScoped<ICartRepository, CartRepository>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<IChatLogRepository, ChatLogRepository>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IProductReviewRepository, ProductReviewRepository>();
            builder.Services.AddScoped<IRoleRepository, RoleRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();

            // Add services to DI container
            builder.Services.AddScoped<IAddressService, AddressService>();
            builder.Services.AddScoped<IAuditLogService, AuditLogService>();
            builder.Services.AddScoped<ICartItemService, CartItemService>();
            builder.Services.AddScoped<ICartService, CartService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IChatLogService, ChatLogService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IProductReviewService, ProductReviewService>();
            builder.Services.AddScoped<IRoleService, RoleService>();
            builder.Services.AddScoped<IUserService, UserService>();


            // Gemini DI
            builder.Services.AddScoped<GeminiClientService>();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = "MyCookieAuth";
                options.DefaultChallengeScheme = "Google";
            })
             .AddCookie("MyCookieAuth", options =>
             {
                 options.LoginPath = "/Account/Login";
                 options.AccessDeniedPath = "/Account/AccessDenied";
                 options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
             })
             .AddGoogle("Google", options =>
             {
                 options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
                 options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
                 options.CallbackPath = "/signin-google"; // hoặc đường dẫn bạn đã khai báo trên Google Console
             });

            builder.Services.AddDistributedMemoryCache(); // Bộ nhớ cache cho session
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // Thời gian timeout session
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });


            // VNpayService DI
            builder.Services.AddScoped<IVnPayService, VnPayService>();


            var app = builder.Build();

            // Initialize DB
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                Console.WriteLine(" ConnectionString: " + builder.Configuration.GetConnectionString("DefaultConnection"));

                DbInitializer.Initialize(context);
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseSession();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
