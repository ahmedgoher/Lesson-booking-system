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
        public async Task<IActionResult> View(string token , int m,int y)
        {
            if(string.IsNullOrEmpty(token)) return NotFound("رابط غير صالح");
            var student = await _context.Students
                .Include(s=>s.AvailableSlot)
                .ThenInclude(a=>a.SlotTimes)
                .Include(s=>s.Place)
                .Include(s=>s.Class)
                .FirstOrDefaultAsync(s => s.ParentAccessToken == token);
            if(student == null) return NotFound("الطالب غير موجود");

            var attendanceList = await _context.Attendances
                .Where(a=>a.StudentId == student.Id && a.Date.Month == m && a.Date.Year == y)
                .Select(a=>new AttendanceDetails
                {
                    Date = a.Date,
                    Status = a.IsPresent == AttendanceStatus.Present ? "حاضر" : (a.IsPresent == AttendanceStatus.Excused ? "متأخر" : "غائب"),
                    HomeworkStatus = a.Homework == HomeworkStatus.Done ? "مكتمل" :(a.Homework == HomeworkStatus.NotDone ? "غير مكتمل" : "-"),
                    Notes = a.Notes ?? "-"
                }).ToListAsync();

            var examsList = await _context.StudentExams
                .Include(se=>se.Exam)
                .Where(se=>se.StudentId ==student.Id && 
                se.Exam.StartExamTime.Month == m 
                && se.Exam.StartExamTime.Year == y)
                .Select(se=>new ExamDetails{
                    ExamName = se.Exam.Title,
                    ExamDate = se.Exam.StartExamTime,
                    TotalMarks = se.Exam.TotalMarks,
                    StudentScore = se.Score,
                    GradeText = se.Score >= (se.Exam.TotalMarks * 0.85) ? "ممتاز" :
                        se.Score >= (se.Exam.TotalMarks * 0.75) ? "جيد جداً" :
                        se.Score >= (se.Exam.TotalMarks * 0.70) ? "جيد" :
                        se.Score >= (se.Exam.TotalMarks * 0.60) ? "مقبول" : "راسب",
                            }).ToListAsync();
            int totalClasses = attendanceList.Count;
            int presentCount = attendanceList.Count(x => x.Status == "حاضر" || x.Status == "متأخر");
            int attendancePercentage = totalClasses > 0 ? (int)((double)presentCount / totalClasses * 100) : 0;

            var viewModel = new MonthlyReportViewModel
            {
                StudentName = student.Name,
                ParentPhone = student.ParentPhone,
                GroupName = student.AvailableSlot?.Group_Name ?? "غير محدد",
                GradeLevel = student.Class?.Name ?? "الصف الدراسي",
                CenterName = student.Place?.Name ?? "المركز التعليمي",
                DaysSchedule = student.AvailableSlot?.SlotTimes.ToList() ?? new List<Slot_time>(),
                Month = m,
                Year = y,
                AttendancePercentage = attendancePercentage,
                AverageExams = examsList.Any() ? $"{examsList.Average(e => e.StudentScore ?? 0):F1} / {examsList.Average(e => e.TotalMarks)}" : "0",
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
