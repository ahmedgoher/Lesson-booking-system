using Al_Muzayyen.Models;
using Al_Muzayyen.Viewmodel;
using Al_Muzayyen.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Al_Muzayyen.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public StudentController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Videos()
        {
            return View();
        }
        public IActionResult Exams()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Attendance()
        {
            var studentIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(studentIdClaim) || !int.TryParse(studentIdClaim, out int studentId))
            {
                return RedirectToAction("login2", "Account");
            }

            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            var attendances = await _context.Attendances
                 .Where(a => a.StudentId == studentId && a.Date.Month == currentMonth && a.Date.Year == currentYear)
                 .OrderByDescending(a => a.Date)
                 .Select(a => new StudentAttendanceVM
                 {
                     Date = a.Date,
                     IsPresent = a.IsPresent,
                     Homework = a.Homework,
                     Notes = a.Notes
                 })
                 .ToListAsync();

            return View(attendances);
        }
        [HttpGet]
        public async Task<IActionResult> FilterAttendance(int month, int year, string status)
        {
            var studentIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(studentIdClaim) || !int.TryParse(studentIdClaim, out int studentId))
            {
                return Unauthorized();
            }

            var query = _context.Attendances
                .Where(a => a.StudentId == studentId && a.Date.Month == month && a.Date.Year == year);

            if (status == "present")
            {
                query = query.Where(a => a.IsPresent == AttendanceStatus.Present);
            }
            else if (status == "absent")
            {
                query = query.Where(a => a.IsPresent == AttendanceStatus.Absent);
            }
            else if (status == "excused")
            {
                query = query.Where(a => a.IsPresent == AttendanceStatus.Excused);
            }

            var result = await query
                .OrderByDescending(a => a.Date)
                .Select(a => new StudentAttendanceVM
                {
                    Date = a.Date,
                    IsPresent = a.IsPresent,
                    Homework = a.Homework,
                    Notes = a.Notes
                })
                .ToListAsync();

            return PartialView("_StudentAttendanceRows", result);
        }
        public IActionResult Materials()
        {
            return View();
        }
        public async Task<IActionResult> Profile()
        {
            var studentIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(studentIdClaim) || !int.TryParse(studentIdClaim, out int studentId))
            {
                return RedirectToAction("login2", "Account");
            }

            var student = await _context.Students
                .Include(s => s.Class)
                .Include(s => s.AvailableSlot)
                .FirstOrDefaultAsync(s => s.Id == studentId);

            if (student == null) return NotFound();

            var model = new StudentProfileVM
            {
                Id = student.Id,
                Name = student.Name,
                StdPhone = student.StdPhone,
                ParentPhone = student.ParentPhone ?? "",
                ClassName = student.Class?.Name ?? "غير محدد",
                GroupName = student.AvailableSlot?.Group_Name ?? "غير محدد"
            };

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateInfo([FromBody] StudentProfileVM model)
        {
            var studentIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(studentIdClaim) || !int.TryParse(studentIdClaim, out int studentId))
            {
                return Json(new { success = false, message = "جلسة العمل انتهت، يرجى تسجيل الدخول مجدداً." });
            }

            var student = await _context.Students.FindAsync(studentId);
            if (student == null) return Json(new { success = false, message = "الطالب غير موجود" });

            // التحقق من عدم تكرار رقم الهاتف مع طالب آخر
            bool phoneExists = await _context.Students.AnyAsync(s => s.StdPhone == model.StdPhone && s.Id != studentId);

            if (phoneExists)
            {
                return Json(new { success = false, message = "رقم الهاتف مستخدم بالفعل لطالب آخر!" });
            }

            student.Name = model.Name;
            student.StdPhone = model.StdPhone;
            student.ParentPhone = model.ParentPhone;

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "تم حفظ البيانات الشخصية بنجاح!" });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePassword([FromBody] ChangePasswordVM model)
        {
            var studentIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(studentIdClaim) || !int.TryParse(studentIdClaim, out int studentId))
            {
                return Json(new { success = false, message = "جلسة العمل انتهت، يرجى تسجيل الدخول مجدداً." });
            }

            var student = await _context.Students.FindAsync(studentId);
            if (student == null) return Json(new { success = false, message = "الطالب غير موجود" });

            // التأكد من صحة كلمة المرور الحالية
            if (student.Password != model.CurrentPassword)
            {
                return Json(new { success = false, message = "كلمة المرور الحالية غير صحيحة!" });
            }

            if (model.NewPassword != model.ConfirmPassword)
            {
                return Json(new { success = false, message = "كلمة المرور الجديدة وتأكيدها غير متطابقين!" });
            }

            // تحديث كلمة المرور
            student.Password = model.NewPassword;
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "تم تغيير كلمة المرور بنجاح!" });
        }

        public IActionResult Account()
        {
            return View();
        }
    }
}
