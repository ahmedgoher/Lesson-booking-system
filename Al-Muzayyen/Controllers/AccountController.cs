using Al_Muzayyen.Models;
using Al_Muzayyen.Viewmodel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity; // 👈 مهم جداً للـ PasswordHasher
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Al_Muzayyen.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        // 👈 تعريف الـ PasswordHasher لفحص أو توليد الهاش
        private readonly IPasswordHasher<object> _passwordHasher;

        public AccountController(AppDbContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<object>();
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
                .FirstOrDefaultAsync(a => a.PhoneNumber == model.PhoneNumber);

            // التحقق من الباسورد المتهيش باستخدام PasswordVerificationResult
            if (admin != null)
            {
                var verificationResult = _passwordHasher.VerifyHashedPassword(admin, admin.Password, model.Password);
                if (verificationResult == PasswordVerificationResult.Success)
                {
                    var adminClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, admin.Id.ToString()),
                        new Claim(ClaimTypes.Name, admin.Name ?? admin.PhoneNumber ?? "الأدمن"),
                        new Claim(ClaimTypes.MobilePhone, admin.PhoneNumber ?? ""),
                        new Claim(ClaimTypes.Role, "Admin")
                    };

                    var adminIdentity = new ClaimsIdentity(adminClaims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = model.RememberMe,
                        ExpiresUtc = model.RememberMe ? DateTimeOffset.UtcNow.AddHours(2) : DateTimeOffset.UtcNow.AddMinutes(30)
                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(adminIdentity), authProperties);
                    return RedirectToAction("Index", "Admin");
                }
            }

            // 2️⃣ فحص الطالب
            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.StdPhone == model.PhoneNumber);

            if (student != null)
            {
                var verificationResult = _passwordHasher.VerifyHashedPassword(student, student.Password, model.Password);
                if (verificationResult == PasswordVerificationResult.Success)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, student.Id.ToString()),
                        new Claim(ClaimTypes.Name, student.Name),
                        new Claim(ClaimTypes.MobilePhone, student.StdPhone),
                        new Claim(ClaimTypes.Role, "Student")
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = model.RememberMe,
                        ExpiresUtc = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(30) : DateTimeOffset.UtcNow.AddHours(2)
                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                    return RedirectToAction("Index", "Home");
                }
            }

            ViewBag.Error = "رقم الهاتف أو كلمة المرور غير صحيحة!";
            return View(model);
        }

        [HttpGet, HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            return RedirectToAction("login2", new { returnUrl });
        }



    }
}