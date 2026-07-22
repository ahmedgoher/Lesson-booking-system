 // قم بتعديل هذا النطاق لحسب مسار الـ DbContext لديك
using Al_Muzayyen.Models;
using Al_Muzayyen.Viewmodel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Al_Muzayyen.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context; // حقن قاعدة البيانات

        public AccountController(IConfiguration configuration, AppDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        // GET: صفحة تسجيل دخول الطالب
        [HttpGet]
        public IActionResult login2()
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("Student"))
            {
                return RedirectToAction("Index", "Home"); // أو توجيهه للوحة الطالب
            }
            return View();
        }

        // POST: عملية تسجيل دخول الطالب والتحقق
        [HttpPost]
        public async Task<IActionResult> login2(StudentLoginVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // 1. البحث عن الطالب برقم الهاتف وكلمة المرور
            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.StdPhone == model.PhoneNumber && s.Password == model.Password);

            if (student != null)
            {
                // 2. تجهيز بيانات الاعتماد (Claims) للطالب
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, student.Id.ToString()),
                    new Claim(ClaimTypes.Name, student.Name),
                    new Claim(ClaimTypes.MobilePhone, student.StdPhone),
                    new Claim(ClaimTypes.Role, "Student")
                };

                var claimsIdentity = new ClaimsIdentity(claims, "StudentAuth");

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe,
                    ExpiresUtc = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(30) : DateTimeOffset.UtcNow.AddHours(2)
                };

                // 3. تسجيل الدخول بالكوكي المخصص للطلاب
                await HttpContext.SignInAsync("StudentAuth", new ClaimsPrincipal(claimsIdentity), authProperties);

                return RedirectToAction("Index", "Home");
            }

            // لو البيانات خطأ
            ViewBag.Error = "رقم الهاتف أو كلمة المرور غير صحيحة!";
            return View(model);
        }

        // تسجيل خروج الطالب
        public async Task<IActionResult> StudentLogout()
        {
            await HttpContext.SignOutAsync("StudentAuth");
            return RedirectToAction("login2");
        }


        // ================= ADMIN LOGIN =================

        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
                return RedirectToAction("Index", "Admin");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var secureUsername = _configuration["AdminSettings:Username"];
            var securePassword = _configuration["AdminSettings:Password"];

            if (model.Username == secureUsername && model.Password == securePassword)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, model.Username),
                    new Claim(ClaimTypes.Role, "Admin")
                };

                var claimsIdentity = new ClaimsIdentity(claims, "AdminAuth");

                await HttpContext.SignInAsync("AdminAuth", new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Index", "Admin");
            }

            TempData["Error"] = "اسم المستخدم أو كلمة المرور غير صحيحة!";
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // مسح كوكيز الطالب وكوكيز الآدمن
            await HttpContext.SignOutAsync("StudentAuth");
            await HttpContext.SignOutAsync("AdminAuth");

            return RedirectToAction("Index", "Home");
        }
        
    }
}