using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Al_Muzayyen.Viewmodel;
using Microsoft.AspNetCore.Mvc;

namespace Al_Muzayyen.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _configuration;
        public AccountController(IConfiguration configuration)
        {
            _configuration= configuration;
        }
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated) return RedirectToAction("Index", "Admin");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // قراءة البيانات الديناميكية من ملف الإعدادات
            var secureUsername = _configuration["AdminSettings:Username"];
            var securePassword = _configuration["AdminSettings:Password"];

            if (model.Username == secureUsername && model.Password == securePassword)
            {
                // كود الـ Claims والـ SignIn الحالية زي ما هي بدون تغيير...
                // 1. إنشاء قائمة بيانات الهوية للمستخدم الحالي
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, model.Username),
                    new Claim(ClaimTypes.Role, "Admin")
                };

                var claimsIdentity = new ClaimsIdentity(claims, "AdminAuth");

                // 2. تفعيل الكوكي في متصفح العميل
                await HttpContext.SignInAsync("AdminAuth", new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Index", "Admin");
            }

            // لو البيانات غلط
            TempData["Error"] = "اسم المستخدم أو كلمة المرور غير صحيحة!";
            return View(model);
        }

        // دالة تسجيل الخروج بالمرة تبرمجها
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("AdminAuth");
            return RedirectToAction("Login");
        }
    }
}