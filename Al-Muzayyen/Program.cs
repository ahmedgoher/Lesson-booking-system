using Al_Muzayyen.Configurations;
using Al_Muzayyen.Models;
using Al_Muzayyen.Repositories;
using Al_Muzayyen.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
//using Al_Muzayyen.Configurations;


namespace Al_Muzayyen
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            // 1. تفعيل خدمات الـ Authentication والـ Cookies
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = "AdminAuth";
                options.DefaultChallengeScheme = "AdminAuth";
            })
 .AddCookie("AdminAuth", options =>
 {
     options.LoginPath = "/Account/Login";

     // 1. وقت انتهاء صلاحية الكوكي (مثلاً 20 دقيقة من الخمول)
     options.ExpireTimeSpan = TimeSpan.FromMinutes(20);

     // 2. تجديد الوقت تلقائياً طالما الآدمن بيتحرك في الموقع (Sliding Expiration)
     options.SlidingExpiration = true;

     // 3. تأمين الكوكي بحيث لا يتم الوصول إليه عبر برمجيات خبيثة (Javascript)
     options.Cookie.HttpOnly = true;

     // 4. جعل الكوكي ينتهي بمجرد قفل المتصفح بالكامل لو الآدمن معلمش على "تذكرني"
     options.Cookie.IsEssential = true;
 });
            // قراءة الكونيكشن استرنج من ملف appsettings.json
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
            });

            // تسجيل الـ Generic Repository
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped(typeof(IGenericService<>), typeof(GenericSevice<>));
            builder.Services.AddScoped<IClassService, ClassService>();
            builder.Services.AddScoped<IPlaceService, PlaceService>();
            builder.Services.AddScoped<ISlotService, SlotService>();
            builder.Services.AddScoped<IBookingService, BookingService>();
            // Repos
            builder.Services.AddScoped<IBookingRepo, BookingRepo>();
            builder.Services.AddScoped<IGroupRepo, GroupRepo>();

            builder.Services.Configure<CloudinarySettings>(
builder.Configuration.GetSection("CloudinarySettings"));

            builder.Services.AddScoped<CloudinaryService>();
            // تسجيل سيرفيس الحجوزات
            builder.Services.AddScoped<IBookingService, BookingService>();
            // تسجيل الـ DbContext داخل خدمات المشروع
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));


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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseRouting();

            // 2. تفعيل جدار الحماية (الترتيب هنا إجباري ومهم جداً!)
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
