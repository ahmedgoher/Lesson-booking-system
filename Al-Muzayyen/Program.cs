//using Al_Muzayyen.Configurations;
//using Al_Muzayyen.Models;
//using Al_Muzayyen.Repositories;
//using Al_Muzayyen.Services;
//using Microsoft.AspNetCore.Authentication.Cookies;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;

//namespace Al_Muzayyen
//{
//    public class Program
//    {
//        public static void Main(string[] args)
//        {
//            var builder = WebApplication.CreateBuilder(args);

//            // إضافة الخدمات إلى الـ Container
//            builder.Services.AddControllersWithViews();

//            // قراءة نص الاتصال
//            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

//            // تسجيل الـ DbContext
//            builder.Services.AddDbContext<AppDbContext>(options =>
//                options.UseSqlServer(connectionString));

//            // 🟢 1. إعدادات Identity
//            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
//            {
//                options.Password.RequireDigit = false;
//                options.Password.RequireLowercase = false;
//                options.Password.RequireUppercase = false;
//                options.Password.RequireNonAlphanumeric = false;
//                options.Password.RequiredLength = 6;
//            })
//            .AddEntityFrameworkStores<AppDbContext>()
//            .AddDefaultTokenProviders();

//            // 🟢 2. تفعيل خدمة التوثيق بنظام الكوكيز الموحد (CookieAuthenticationDefaults)
//            builder.Services.AddAuthentication(options =>
//            {
//                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//            })
//            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
//            {
//                options.Cookie.Name = "AlMuzayyenAuthCookie";
//                options.LoginPath = "/Account/login2";        // المسار الموحد لصفحة تسجيل الدخول
//                options.AccessDeniedPath = "/Account/login2";  // تحويل من لا يملك صلاحية إلى login2
//                options.ExpireTimeSpan = TimeSpan.FromDays(30);
//                options.SlidingExpiration = true;
//                options.Cookie.HttpOnly = true;
//                options.Cookie.IsEssential = true;
//            });

//            builder.Services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(options =>
//            {
//                options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
//            });

//            // Repositories & Services
//            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
//            builder.Services.AddScoped(typeof(IGenericService<>), typeof(GenericSevice<>));
//            builder.Services.AddScoped<IClassService, ClassService>();
//            builder.Services.AddScoped<IPlaceService, PlaceService>();
//            builder.Services.AddScoped<ISlotService, SlotService>();
//            builder.Services.AddScoped<IBookingService, BookingService>();
//            builder.Services.AddScoped<IBookingRepo, BookingRepo>();
//            builder.Services.AddScoped<IGroupRepo, GroupRepo>();
//            builder.Services.AddScoped<IClassService, ClassService>();
//            builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
//            builder.Services.AddScoped<IGroupRequestRepo, GroupRequestRepo>();
//            builder.Services.AddScoped<IStudentRepository, StudentRepository>();
//            builder.Services.AddScoped<IStudentService, StudentService>();
//            builder.Services.AddScoped<IStudentAnswerRepository, StudentAnswerRepository>();
//            builder.Services.AddScoped<IStudentAnswerService, StudentAnswerService>();
//            builder.Services.AddScoped<IQuestionService, QuestionService>();
//            // أو AddTransient / AddSingleton حسب نوع الخدمة لديك
//            // Repositories
//            builder.Services.AddScoped<IExamRepository, ExamRepository>();

//            // Services
//            builder.Services.AddScoped<IExamService, ExamService>();
//            builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));
//            builder.Services.AddScoped<CloudinaryService>();

//            var app = builder.Build();

//            if (!app.Environment.IsDevelopment())
//            {
//                app.UseExceptionHandler("/Home/Error");
//                app.UseHsts();
//            }

//            app.UseHttpsRedirection();
//            app.UseStaticFiles();
//            app.UseRouting();

//            // جدار الحماية (الترتيب مهم)
//            app.UseAuthentication();
//            app.UseAuthorization();

//            app.MapStaticAssets();
//            app.MapControllerRoute(
//                name: "default",
//                pattern: "{controller=Home}/{action=Index}/{id?}")
//                .WithStaticAssets();

//            // 🟢 إنشاء أول أدمن تلقائياً في قاعدة البيانات إذا كان الجدول فارغاً
//            using (var scope = app.Services.CreateScope())
//            {
//                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

//                if (!context.Admins.Any())
//                {
//                    context.Admins.Add(new Al_Muzayyen.Models.Admin
//                    {
//                        Name = "الأستاذ عبد الفتاح المزين",
//                        PhoneNumber = "01000000000", // رقم الأدمن للاختبار
//                        Password = "123456"          // كلمة المرور
//                    });

//                    context.SaveChanges();
//                }
//            }

//            app.Run();
//        }
//    }
//}
using Al_Muzayyen.Configurations;
using Al_Muzayyen.Models;
using Al_Muzayyen.Repositories;
using Al_Muzayyen.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace Al_Muzayyen
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // إضافة الخدمات إلى الـ Container
            builder.Services.AddControllersWithViews();

            // قراءة نص الاتصال
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            // تسجيل الـ DbContext
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));

            // 🟢 تسجيل خدمات Identity Core (بدون تغيير التوثيق الافتراضي الخاص بك)
            builder.Services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
            })
            .AddEntityFrameworkStores<AppDbContext>();

            // 🟢 التوثيق الموحد الخاص بك عبر الكوكيز
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.Name = "AlMuzayyenAuthCookie";
                    options.LoginPath = "/Account/login2";
                    options.AccessDeniedPath = "/Account/login2";
                    options.ExpireTimeSpan = TimeSpan.FromDays(30);
                    options.SlidingExpiration = true;
                    options.Cookie.HttpOnly = true;
                    options.Cookie.IsEssential = true;
                });
            builder.Services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
            });

            // Repositories & Services
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped(typeof(IGenericService<>), typeof(GenericSevice<>));
            builder.Services.AddScoped<IClassService, ClassService>();
            builder.Services.AddScoped<IPlaceService, PlaceService>();
            builder.Services.AddScoped<ISlotService, SlotService>();
            builder.Services.AddScoped<IBookingService, BookingService>();
            builder.Services.AddScoped<IBookingRepo, BookingRepo>();
            builder.Services.AddScoped<IGroupRepo, GroupRepo>();
            builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
            builder.Services.AddScoped<IGroupRequestRepo, GroupRequestRepo>();
            builder.Services.AddScoped<IStudentRepository, StudentRepository>();
            builder.Services.AddScoped<IStudentService, StudentService>();
            builder.Services.AddScoped<IStudentAnswerRepository, StudentAnswerRepository>();
            builder.Services.AddScoped<IStudentAnswerService, StudentAnswerService>();
            builder.Services.AddScoped<IQuestionService, QuestionService>();
            builder.Services.AddScoped<IExamRepository, ExamRepository>();
            builder.Services.AddScoped<IExamService, ExamService>();

            builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));
            builder.Services.AddScoped<CloudinaryService>();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            // 🟢 الترتيب مهم جداً
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            // 🟢 إنشاء أول أدمن تلقائياً في قاعدة البيانات إذا كان الجدول فارغاً
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                if (!context.Admins.Any())
                {
                    // إنشاء كائن الـ Hasher
                    var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<Al_Muzayyen.Models.Admin>();

                    var admin = new Al_Muzayyen.Models.Admin
                    {
                        Name = "الأستاذ عبد الفتاح المزين",
                        PhoneNumber = "01000000000"
                    };

                    // تشفير كلمة المرور وتعيينها
                    admin.Password = hasher.HashPassword(admin, "123456");

                    context.Admins.Add(admin);
                    context.SaveChanges();
                }
            }

            app.Run();
        }
    }
}