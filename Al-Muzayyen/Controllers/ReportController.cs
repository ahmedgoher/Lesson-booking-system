using Al_Muzayyen.Models;
using Al_Muzayyen.Viewmodel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Al_Muzayyen.Controllers
{
    public class ReportController : Controller
    {
        private readonly AppDbContext _context;

        public ReportController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> View(string token, int? m = null, int? y = null)
        {
            if (string.IsNullOrEmpty(token)) return NotFound("رابط غير صالح");

            // 🎯 التحديد التلقائي للشهر والسنة الحاليين إذا لم يتم تمريرهما في اللينك
            int selectedMonth =  DateTime.Now.Month;
            int selectedYear = DateTime.Now.Year;

            var student = await _context.Students
                .Where(s => s.IsActive == true)
                .Include(s => s.AvailableSlot)
                .ThenInclude(a => a.SlotTimes)
                .Include(s => s.Place)
                .Include(s => s.Class)
                .FirstOrDefaultAsync(s => s.ParentAccessToken == token);

            if (student == null) return NotFound("الطالب غير موجود");

            // تحديد نطاق الشهر المختار (أو الحالي تلقائياً)
            var startDate = new DateTime(selectedYear, selectedMonth, 1);
            var endDate = startDate.AddMonths(1);

            // 1. سجلات الحضور
            var attendanceList = await _context.Attendances
                .Where(a => a.StudentId == student.Id && a.Date >= startDate && a.Date < endDate)
                .Select(a => new AttendanceDetails
                {
                    Date = a.Date,
                    Status = a.IsPresent == AttendanceStatus.Present ? "حاضر" : (a.IsPresent == AttendanceStatus.Excused ? "متأخر" : "غائب"),
                    HomeworkStatus = a.Homework == HomeworkStatus.Done ? "مكتمل" : (a.Homework == HomeworkStatus.NotDone ? "غير مكتمل" : "-"),
                    Notes = a.Notes ?? "-"
                }).ToListAsync();

            // 2. سجلات الامتحانات
            var examsList = await _context.StudentExams
                .Include(se => se.Exam)
                .Where(se => se.StudentId == student.Id
                          && (se.Exam.IsPaperExam ? (se.Exam.ExamDate >= startDate && se.Exam.ExamDate < endDate)
                                                  : (se.Exam.StartExamTime >= startDate && se.Exam.StartExamTime < endDate)))
                .Select(se => new ExamDetails
                {
                    ExamName = se.Exam.Title,
                    ExamDate = se.Exam.IsPaperExam && se.Exam.ExamDate.HasValue ? se.Exam.ExamDate.Value : se.Exam.StartExamTime,
                    TotalMarks = se.Exam.TotalMarks,
                    StudentScore = se.Score,
                    GradeText = se.Score >= (se.Exam.TotalMarks * 0.85) ? "ممتاز" :
                                se.Score >= (se.Exam.TotalMarks * 0.75) ? "جيد جداً" :
                                se.Score >= (se.Exam.TotalMarks * 0.70) ? "جيد" :
                                se.Score >= (se.Exam.TotalMarks * 0.60) ? "مقبول" : "راسب",
                    Notes = "-"
                }).ToListAsync();

            int totalClasses = attendanceList.Count;
            int presentCount = attendanceList.Count(x => x.Status == "حاضر" || x.Status == "متأخر");
            int attendancePercentage = totalClasses > 0 ? (int)((double)presentCount / totalClasses * 100) : 0;

            double avgScore = examsList.Any() ? examsList.Average(e => e.StudentScore ?? 0) : 0;
            double avgTotal = examsList.Any() ? examsList.Average(e => e.TotalMarks) : 0;

            var viewModel = new MonthlyReportViewModel
            {
                StudentName = student.Name,
                ParentPhone = student.ParentPhone,
                GroupName = student.AvailableSlot?.Group_Name ?? "غير محدد",
                GradeLevel = student.Class?.Name ?? "الصف الدراسي",
                CenterName = student.Place?.Name ?? "المركز التعليمي",
                DaysSchedule = student.AvailableSlot?.SlotTimes.ToList() ?? new List<Slot_time>(),
                Month = selectedMonth,
                Year = selectedYear,
                AttendancePercentage = attendancePercentage,
                AverageExams = examsList.Any() ? $"{avgScore:F1} / {avgTotal:F0}" : "0",
                HomeworkCompletion = $"{attendanceList.Count(x => x.HomeworkStatus == "مكتمل")} / {totalClasses} حصص",
                GeneralEvaluation = attendancePercentage >= 85 ? "ممتاز" :
                                    attendancePercentage >= 75 ? "جيد جداً" :
                                    attendancePercentage >= 70 ? "جيد" :
                                    attendancePercentage >= 60 ? "مقبول" : "ضعيف",
                TeacherNotes = student.Notes ?? "استمر في الأداء المتميز.",
                AttendanceRecord = attendanceList,
                ExamRecord = examsList
            };

            return View(viewModel);
        }
    }
}
