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
            var studentIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(studentIdClaim) || !int.TryParse(studentIdClaim, out int studentId))
            {
                return RedirectToAction("login2", "Account");
            }
            var student = _context.Students
                .Include(s => s.AvailableSlot)
                .FirstOrDefault(s => s.Id == studentId);

            if (student == null || student.SlotId == null)
            {
                return View(new StudentMatrialVM());
            }
            var videos = _context.Materials
                .Where(m => m.SlotId == student.SlotId && m.Type == MaterialType.VideoLink)
                .OrderByDescending(v => v.CreatedAt)
                .ToList();
            var viewModel = new StudentMatrialVM
            {
                GroupName = student.AvailableSlot?.Group_Name ?? "مجموعتي",
                Matrials = videos
            };
            return View(viewModel);
        }
        public async Task<IActionResult> Exams()
        {
            // 1. جلب معرف المستخدم الحالي من الـ Claim
            var userClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userClaim))
            {
                return RedirectToAction("login2", "Account");
            }

            // 2. البحث عن الطالب سواء كان الـ Claim هو UserId أو Student Id
            Student? student = null;

            if (int.TryParse(userClaim, out int studentId))
            {
                student = await _context.Students.FirstOrDefaultAsync(s => s.Id == studentId);
            }
            else
            {
                student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == userClaim);
            }

            if (student == null)
            {
                return View(new List<StudentExamViewModel>());
            }

            int currentStudentSlotId = student.SlotId;
            int currentStudentId = student.Id;

            // 3. جلب الامتحانات الموجهة لمجموعة الطالب عبر الجدول الوسيط ExamGroups
            var exams = await _context.Exams
                .Where(e => e.IsActive && e.ExamGroups.Any(eg => eg.SlotId == currentStudentSlotId))
                .Select(e => new StudentExamViewModel
                {
                    ExamId = e.Id,
                    ExamTitle = e.Title,
                    Date = e.StartExamTime,
                    TotalMarks = e.TotalMarks,

                    // التأكد هل أدى الطالب هذا الامتحان قبل ذلك؟
                    IsCompleted = e.StudentExams.Any(se => se.StudentId == currentStudentId),

                    // جلب درجة الطالب إن وجدت
                    Score = e.StudentExams
                             .Where(se => se.StudentId == currentStudentId)
                             .Select(se => (double?)se.Score)
                             .FirstOrDefault()
                })
                .ToListAsync();

            return View(exams);
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
            var studentIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(studentIdClaim) || !int.TryParse(studentIdClaim, out int studentId))
            {
                return RedirectToAction("login2", "Account");
            }
            var student = _context.Students
                .Include(s => s.AvailableSlot)
                .FirstOrDefault(s => s.Id == studentId);
            if (student == null)
            {
                return View(new StudentMatrialVM());
            }

            var materials = _context.Materials
                .Where(m => m.SlotId == student.SlotId && m.Type == MaterialType.PDF)
                .OrderByDescending(v => v.CreatedAt)
                .ToList();
            var viewModel = new StudentMatrialVM
            {
                GroupName = student.AvailableSlot?.Group_Name ?? "مجموعتي",
                Matrials = materials
            };
            return View(viewModel);
        }
        [HttpGet]
        public async Task<JsonResult> GetSlots(int classId, int placeId)
        {
            var studentIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(studentIdClaim) || !int.TryParse(studentIdClaim, out int studentId))
            {
                return Json(new { success = false, message = "Unauthorized" });
            }
            var student = await _context.Students
                .Include(s => s.Class)
                .Include(s => s.AvailableSlot)
                .FirstOrDefaultAsync(s => s.Id == studentId);

            var slots = await _context.Available_Slots
                .Where(s => s.ClassId == classId && s.PlaceId == placeId && s.State == "Active" && s.Id != student.SlotId)
                .Select(s => new { id = s.Id, name = s.Group_Name })
                .ToListAsync();

            return Json(slots);
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
            ViewBag.Classes = await _context.Classes.ToListAsync();
            ViewBag.Places = await _context.Places
                .ToListAsync();
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
        [HttpPost]
        public async Task<IActionResult> SubmitGroupChangeRequest([FromBody] ChangeGroupVM vm)
        {
            try
            {
                var studentIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(studentIdClaim) || !int.TryParse(studentIdClaim, out int studentId))
                {
                    return Json(new { success = false, message = "جلسة العمل انتهت، يرجى تسجيل الدخول مجدداً." });
                }
                var student = await _context.Students.FindAsync(studentId);
                if(student == null)
                {
                    return Json(new { success = false, message = "الطالب غير موجود" });
                }
                if(student.SlotId == vm.RequestedSlotId)
                {
                    return Json(new { success = false, message = "أنت بالفعل مسجل في هذه المجموعة!" });
                }
                var existingRequest = await _context.GroupChangeRequests
                    .FirstOrDefaultAsync(r => r.StudentId == studentId && r.Status == RequestStatus.Pending);
                if (existingRequest != null)
                {
                    return Json(new { success = false, message = "لديك طلب تغيير مجموعة قيد الانتظار بالفعل. يرجى انتظار الرد." });
                }
                var newRequest = new GroupChangeRequest
                {
                    StudentId = studentId,
                    RequestSlotId = vm.RequestedSlotId,
                    Reason = vm.Reason,
                    Status = RequestStatus.Pending,
                    RequestDate = DateTime.Now
                };
                _context.GroupChangeRequests.Add(newRequest);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "تم إرسال طلبك بنجاح، يرجى انتظار موافقة الإدارة." });

            }
            catch (Exception)
            {
                return Json(new { success = false, message = "حدث خطأ أثناء إرسال الطلب. حاول مرة أخرى." });

            }
        }

        public IActionResult Account()
        {
            return View();
        }
    }
}
