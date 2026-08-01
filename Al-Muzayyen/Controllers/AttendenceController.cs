using Al_Muzayyen.Models;
using Al_Muzayyen.Viewmodel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Al_Muzayyen.Controllers
{
    public class AttendenceController : Controller
    {
        private readonly AppDbContext _context;

        public AttendenceController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Classes = new SelectList(_context.Classes, "Id", "Name");
            ViewBag.Places = new SelectList(_context.Places, "Id", "Name");
            return View(new AttendenceVM());
        }

        [HttpGet]
        public async Task<JsonResult> GetSlots(int classId, int placeId)
        {
            var slots = await _context.Available_Slots
                .Where(s => s.ClassId == classId && s.PlaceId == placeId && s.State == "Active")
                .Select(s => new { id = s.Id, name = s.Group_Name })
                .ToListAsync();

            return Json(slots);
        }

        [HttpGet]
        public async Task<IActionResult> FetchStudents(int slotId, DateTime date)
        {
            var students = await _context.Students
                .Where(s => s.SlotId == slotId && s.CreatedAt < date && s.IsActive == true)
                .ToListAsync();

            // معرفة أحدث تاريخ تم تسجيل غياب فيه قبل التاريخ المحدد لهذه المجموعة
            var lastClassDate = await _context.Attendances
                .Where(a => a.SlotId == slotId && a.Date < date.Date)
                .OrderByDescending(a => a.Date)
                .Select(a => a.Date)
                .FirstOrDefaultAsync();

            var studentRows = new List<StudentAttendanceRowVM>();

            foreach (var std in students)
            {
                // تم تعديل الشرط هنا إلى Absent للتأكد من حالة الغياب
                bool wasAbsent = false;
                if (lastClassDate != default)
                {
                    wasAbsent = await _context.Attendances.AnyAsync(a =>
                        a.StudentId == std.Id &&
                        a.SlotId == slotId &&
                        a.Date == lastClassDate &&
                        a.IsPresent == AttendanceStatus.Absent);
                }

                // البحث عن حضور مسجل مسبقاً لهذا اليوم
                var existingRecord = await _context.Attendances.FirstOrDefaultAsync(a =>
                    a.StudentId == std.Id &&
                    a.SlotId == slotId &&
                    a.Date.Date == date.Date);

                studentRows.Add(new StudentAttendanceRowVM
                {
                    StudentId = std.Id,
                    StudentName = std.Name,
                    StudentPhone = std.StdPhone,
                    WasAbsentLastTime = wasAbsent,
                    IsPresent = existingRecord?.IsPresent ?? AttendanceStatus.Present,
                    Homework = existingRecord?.Homework ?? HomeworkStatus.Done,
                    Notes = existingRecord?.Notes
                });
            }

            return PartialView("_StudentsAttendanceTable", studentRows);
        }

        [HttpPost]
        public async Task<IActionResult> SaveAttendance([FromBody] AttendenceVM model)
        {
            if (model == null || model.Students == null || !model.Students.Any())
                return BadRequest("لا توجد بيانات للحفظ");

            foreach (var studentRow in model.Students)
            {
                var existingRecord = await _context.Attendances.FirstOrDefaultAsync(a =>
                    a.StudentId == studentRow.StudentId &&
                    a.SlotId == model.SlotId && // تعديل ربط الـ SlotId الصحيح
                    a.Date.Date == model.Date.Date);

                if (existingRecord != null)
                {
                    // تحديث
                    existingRecord.IsPresent = studentRow.IsPresent;
                    existingRecord.Homework = studentRow.Homework;
                    existingRecord.Notes = studentRow.Notes;
                }
                else
                {
                    // إدراج سجل جديد
                    var newAttendance = new Attendance
                    {
                        StudentId = studentRow.StudentId,
                        SlotId = model.SlotId,
                        Date = model.Date.Date,
                        IsPresent = studentRow.IsPresent,
                        Homework = studentRow.Homework,
                        Notes = studentRow.Notes
                    };
                    _context.Attendances.Add(newAttendance);
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "تم حفظ الغياب والواجبات بنجاح" });
        }
    }
}