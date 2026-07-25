

using Al_Muzayyen.Models;
using Al_Muzayyen.Services;
using Al_Muzayyen.Viewmodel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
namespace Al_Muzayyen.Controllers
{
    [Authorize]
    public class ExamsController : Controller
    {
        private readonly IExamService _examService;
        private readonly IQuestionService _questionService;
        private readonly IStudentService _studentService;
        private readonly IStudentAnswerService _studentAnswerService;
        private readonly AppDbContext _context;
        

        public ExamsController(
            IExamService examService,
            IQuestionService questionService,
            IStudentService studentService,
            IStudentAnswerService studentAnswerService,
            AppDbContext context)
        {
            _examService = examService;
            _questionService = questionService;
            _studentService = studentService;
            _studentAnswerService = studentAnswerService;
            _context= context;
        }

        public async Task<IActionResult> Start(int id)
        {
            // 1️⃣ جلب بيانات الامتحان
            var exam = await _examService.GetExamByIdAsync(id);

            if (exam == null)
                return NotFound();

            if (!exam.IsActive)
                return Content("هذا الامتحان غير مفعل");

            if (DateTime.Now < exam.StartExamTime)
                return Content("الامتحان لم يبدأ بعد");

            if (DateTime.Now > exam.EndExamTime)
                return Content("انتهى موعد الامتحان");

            // 2️⃣ جلب UserId الخاص بـ Identity والتحقق منه
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("login2", "Account");

            var student = await _studentService.GetByUserIdAsync(userId);

            if (student == null)
            {
                return Content($"حسابك الحالي (UserId: {userId}) غير مسجل كـ طالب في النظام.");
            }

            // 3️⃣ التحقق من المحاولات السابقة والوقت المنقضي (Auto Submit On Disconnect)
            var existingStudentExam = await _context.StudentExams
                .Where(se => se.ExamId == id && se.StudentId == student.Id)
                .OrderByDescending(se => se.StartedAt)
                .FirstOrDefaultAsync();

            if (existingStudentExam != null)
            {
                // 🟢 أ) إذا كانت المحاولة سارية ولكن الطالب خرج أو انقطع عنه النت وانتهى الوقت
                if (!existingStudentExam.IsSubmitted)
                {
                    var timeElapsed = DateTime.Now - existingStudentExam.StartedAt;

                    // إذا تجاوز الوقت المسموح للامتحان
                    if (timeElapsed.TotalMinutes >= exam.DurationMinutes)
                    {
                        // إغلاق الامتحان وتسليم الإجابات المحفوظة تلقائياً
                        var totalScore = await _context.StudentAnswers
                            .Where(sa => sa.StudentExamId == existingStudentExam.Id && sa.IsCorrect)
                            .SumAsync(sa => sa.EarnedMarks);

                        existingStudentExam.Score = totalScore;
                        existingStudentExam.IsSubmitted = true;
                        existingStudentExam.SubmittedAt = existingStudentExam.StartedAt.AddMinutes(exam.DurationMinutes);

                        await _context.SaveChangesAsync();

                        TempData["ErrorMessage"] = "انتهى وقت الامتحان أثناء غيابك، وتم تسليم إجاباتك المسجلة تلقائياً.";
                        return RedirectToAction("Exam", "Student");
                    }
                }

                // 🟢 ب) التحقق من تجاوز عدد المحاولات المسموح بها (MaxAttempts)
                int completedAttempts = await _context.StudentExams
                    .CountAsync(se => se.ExamId == id && se.StudentId == student.Id && se.IsSubmitted);

                if (completedAttempts >= exam.MaxAttempts)
                {
                    TempData["ErrorMessage"] = "لقد استنفذت جميع المحاولات المسموح بها لهذا الامتحان.";
                    return RedirectToAction("Exam", "Student");
                }
            }

            // 4️⃣ بدء المحاولة وتسجيل البداية
            var studentExam = await _examService.StartExamAsync(id, student.Id);
            var questions = await _questionService.GetStudentQuestionsAsync(id);

            ViewBag.ExamId = exam.Id;
            ViewBag.ExamTitle = exam.Title;
            ViewBag.Duration = exam.DurationMinutes;

            return View(questions);
        }
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveAnswer([FromBody] SaveAnswerViewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Json(new { success = false, message = "غير مسموح، يرجى تسجيل الدخول." });

            var student = await _studentService.GetByUserIdAsync(userId);

            if (student == null)
                return Json(new { success = false, message = "بيانات الطالب غير موجودة." });

            var studentExam = await _examService.StartExamAsync(model.ExamId, student.Id);

            await _studentAnswerService.SaveAnswerAsync(
                studentExam.Id,
                model.QuestionId,
                model.OptionId);

            return Ok(new { success = true });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(int examId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("login2", "Account");

            var student = await _studentService.GetByUserIdAsync(userId);

            if (student == null)
                return Unauthorized();

            // 1️⃣ جلب محاولة الامتحان الحالية للطالب
            var studentExam = await _context.StudentExams
                .FirstOrDefaultAsync(se => se.ExamId == examId && se.StudentId == student.Id && !se.IsSubmitted);

            if (studentExam != null)
            {
                // 2️⃣ حساب مجموع الدرجات المكتسبة من جدول الإجابات
                var totalScore = await _context.StudentAnswers
                    .Where(sa => sa.StudentExamId == studentExam.Id && sa.IsCorrect)
                    .SumAsync(sa => sa.EarnedMarks);

                // 3️⃣ تحديث سجل المحاولة في قاعدة البيانات
                studentExam.Score = totalScore;         // إعطاء الدرجة الفعلية
                studentExam.IsSubmitted = true;         // تعليم الامتحان كـ "تم التسليم"
                studentExam.SubmittedAt = DateTime.Now; // تسجيل وقت التسليم
                studentExam.EndTime = DateTime.Now;

                await _context.SaveChangesAsync();
            }

            // 4️⃣ التحويل لصفحة النتيجة مع تمرير رقم الامتحان أو رقم المحاولة
            return RedirectToAction("Exams", "Student");
        }

        [HttpGet]
        public async Task<IActionResult> Result(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("login2", "Account");

            var student = await _studentService.GetByUserIdAsync(userId);
            if (student == null) return Unauthorized();

            // 1️⃣ جلب الامتحان مع خصائصه (ShowResult & AllowReview)
            var exam = await _examService.GetExamByIdAsync(id);
            if (exam == null) return NotFound();

            // 2️⃣ جلب محاولة الطالب الأخيرة
            var studentExam = await _context.StudentExams
                .FirstOrDefaultAsync(se => se.ExamId == id && se.StudentId == student.Id && se.IsSubmitted);

            if (studentExam == null)
                return RedirectToAction("Exam", "Student");

            // 3️⃣ إرسال الإعدادات للـ View عبر ViewBag
            ViewBag.ShowResult = exam.ShowResult;
            ViewBag.AllowReview = exam.AllowReview;
            ViewBag.ExamTitle = exam.Title;
            ViewBag.ExamId = exam.Id;

            return View(studentExam);
        }
        [HttpGet]
        public async Task<IActionResult> Review(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var student = await _studentService.GetByUserIdAsync(userId);
            if (student == null) return Unauthorized();

            // 1️⃣ جلب بيانات الامتحان
            var exam = await _examService.GetExamByIdAsync(id);
            if (exam == null) return NotFound();

            // 🛡️ حماية: منع المراجعة إذا كانت الخاصية False في قاعدة البيانات
            if (!exam.AllowReview)
            {
                TempData["ErrorMessage"] = "غير مسموح بمراجعة إجابات هذا الامتحان.";
                return RedirectToAction("Exam", "Student");
            }

            // 2️⃣ جلب محاولة الطالب مع إجاباته
            var studentExam = await _context.StudentExams
                .FirstOrDefaultAsync(se => se.ExamId == id && se.StudentId == student.Id && se.IsSubmitted);

            if (studentExam == null) return NotFound();

            // 3️⃣ جلب أسئلة الامتحان بالإجابات المسجلة
            var questions = await _questionService.GetStudentQuestionsAsync(id);
            var studentAnswers = await _context.StudentAnswers
                .Where(sa => sa.StudentExamId == studentExam.Id)
                .ToListAsync();

            ViewBag.StudentAnswers = studentAnswers;
            ViewBag.ExamTitle = exam.Title;

            return View(questions);
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> PaperExam()
        {
            return View();

        }
    }
}