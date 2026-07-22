using Al_Muzayyen.Configurations;
using Al_Muzayyen.Models;
using Al_Muzayyen.Repositories;
using Al_Muzayyen.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Al_Muzayyen
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // 1. تفعيل خدمات الـ Authentication وتحديد كوكيز لكل من (الآدمن والطالب)
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = "StudentAuth"; // المخطط الافتراضي لزوار وطالب الموقع
                options.DefaultChallengeScheme = "StudentAuth";
            })
            .AddCookie("AdminAuth", options =>
            {
                options.Cookie.Name = "AdminAuthCookie";
                options.LoginPath = "/Account/Login";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
                options.SlidingExpiration = true;
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            })
            .AddCookie("StudentAuth", options =>
            {
                options.Cookie.Name = "StudentAuthCookie";
                options.LoginPath = "/Account/login2"; // صفحة دخول الطالب
                options.ExpireTimeSpan = TimeSpan.FromDays(7); // مدة حفظ الجلسة للطالب
                options.SlidingExpiration = true;
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // قراءة الكونيكشن استرنج من ملف appsettings.json
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            builder.Services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
            });

            // تسجيل الـ Generic Repository & Services
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped(typeof(IGenericService<>), typeof(GenericSevice<>));

            // تسجيل الخدمات (Services)
            builder.Services.AddScoped<IClassService, ClassService>();
            builder.Services.AddScoped<IPlaceService, PlaceService>();
            builder.Services.AddScoped<ISlotService, SlotService>();
            builder.Services.AddScoped<IBookingService, BookingService>();

            // تسجيل المستودعات (Repos)
            builder.Services.AddScoped<IBookingRepo, BookingRepo>();
            builder.Services.AddScoped<IGroupRepo, GroupRepo>();

            // إعدادات Cloudinary
            builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));
            builder.Services.AddScoped<CloudinaryService>();

            // تسجيل الـ DbContext داخل خدمات المشروع
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));

            // إعدادات Identity
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseRouting();

            // 2. تفعيل جدار الحماية (الترتيب مهم)
            app.UseAuthentication(); // التحقق من الهوية
            app.UseAuthorization();  // التحقق من الصلاحيات

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}