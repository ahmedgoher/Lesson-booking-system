using Al_Muzayyen.Models;
using Al_Muzayyen.Viewmodel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Al_Muzayyen.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult login2()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Admin"))
                    return RedirectToAction("Index", "Admin");

                if (User.IsInRole("Student"))
                    return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> login2(StudentLoginVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // 1️⃣ فحص الآدمن
            var admin = await _context.Admins
                .FirstOrDefaultAsync(a => a.PhoneNumber == model.PhoneNumber && a.Password == model.Password);

            if (admin != null)
            {
                var adminClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, admin.Id.ToString()),
                    new Claim(ClaimTypes.Name, admin.Name ?? admin.PhoneNumber ?? "الأدمن"),
                    new Claim(ClaimTypes.MobilePhone, admin.PhoneNumber ?? ""),
                    new Claim(ClaimTypes.Role, "Admin") // 👈 إضافة رتبة آدمن
                };

                // استخدام المخطط الموحد Cookies
                var adminIdentity = new ClaimsIdentity(adminClaims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe,
                    ExpiresUtc = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(30) : DateTimeOffset.UtcNow.AddHours(2)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(adminIdentity),
                    authProperties);

                return RedirectToAction("Index", "Admin");
            }

            // 2️⃣ فحص الطالب
            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.StdPhone == model.PhoneNumber && s.Password == model.Password);

            if (student != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, student.Id.ToString()),
                    new Claim(ClaimTypes.Name, student.Name),
                    new Claim(ClaimTypes.MobilePhone, student.StdPhone),
                    new Claim(ClaimTypes.Role, "Student") // 👈 إضافة رتبة طالب
                };

                // استخدام نفس المخطط الموحد Cookies
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe,
                    ExpiresUtc = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(30) : DateTimeOffset.UtcNow.AddHours(2)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "رقم الهاتف أو كلمة المرور غير صحيحة!";
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // مسح كوكيز التوثيق الموحدة
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
        // 🟢 تحويل أي طلب قديم لـ Login إلى login2 مع الحفاظ على رابط العودة ReturnUrl
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            return RedirectToAction("login2", new { returnUrl });
        }
    }
}