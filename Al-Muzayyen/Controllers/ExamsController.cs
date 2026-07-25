

using Al_Muzayyen.Models;
using Al_Muzayyen.Services;
using Al_Muzayyen.Viewmodel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
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
            { TempData["Error"] = "هذا الامتحان غير مفعل";
            return RedirectToAction("Exams", "Student"); }

            if (DateTime.Now < exam.StartExamTime)
            {
                TempData["Error"] = "الامتحان لم يبدأ بعد";
                return RedirectToAction("Exams", "Student");
            }

            if (DateTime.Now > exam.EndExamTime)
            { TempData["Error"] = "انتهى موعد الامتحان";
            return RedirectToAction("Exams", "Student"); }

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
                        return RedirectToAction("Exams", "Student");
                    }
                }

                // 🟢 ب) التحقق من تجاوز عدد المحاولات المسموح بها (MaxAttempts)
                int completedAttempts = await _context.StudentExams
                    .CountAsync(se => se.ExamId == id && se.StudentId == student.Id && se.IsSubmitted);

                if (completedAttempts >= exam.MaxAttempts)
                {
                    TempData["ErrorMessage"] = "لقد استنفذت جميع المحاولات المسموح بها لهذا الامتحان.";
                    return RedirectToAction("Exams", "Student");
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
            TempData["SuccessMessage"] = "تم تسليم الامتحان بنجاح!";
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
                return RedirectToAction("Exams", "Student");

            // 3️⃣ إرسال الإعدادات للـ View عبر ViewBag
            ViewBag.ShowResult = exam.ShowResult;
            ViewBag.AllowReview = exam.AllowReview;
            ViewBag.ExamTitle = exam.Title;
            ViewBag.ExamId = exam.Id;

            return View(studentExam);
        }
        //[HttpGet]
        //public async Task<IActionResult> Review(int examId)
        //{
        //    // 1️⃣ جلب UserId الخاص بالطالب
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    if (string.IsNullOrEmpty(userId))
        //        return RedirectToAction("login2", "Account");

        //    var student = await _studentService.GetByUserIdAsync(userId);
        //    if (student == null) return Unauthorized();

        //    // 2️⃣ جلب الامتحان والتحقق من وجوده
        //    var exam = await _context.Exams.FirstOrDefaultAsync(e => e.Id == examId);
        //    if (exam == null) return NotFound();

        //    // 3️⃣ جلب محاولة الطالب الأخيرة المعتمدة
        //    var submission = await _context.StudentExams
        //        .Where(se => se.ExamId == examId && se.StudentId == student.Id)
        //        .OrderByDescending(se => se.StartedAt)
        //        .FirstOrDefaultAsync();

        //    // الشرط الأول: هل أتم الطالب تسليم الامتحان؟
        //    bool hasSubmitted = submission != null && submission.IsSubmitted;

        //    // الشرط الثاني: هل انتهى وقت نهاية الامتحان المحدد للنظام بالكامل؟
        //    bool isExamEnded = DateTime.Now >= exam.EndExamTime;

        //    // ⛔ نمنع المراجعة ما لم يتحقق الشرطان
        //    if (!hasSubmitted || !isExamEnded)
        //    {
        //        TempData["ErrorMessage"] = "لا يمكنك عرض نموذج الإجابة إلا بعد تسليم الامتحان وانتهاء الموعد الرسمي المخصص للامتحان بالكامل.";
        //        return RedirectToAction("Exams", "Student");
        //    }

        //    // 4️⃣ جلب الأسئلة مع الاختيارات
        //    var questions = await _context.Questions
        //        .Include(q => q.Options)
        //        .Where(q => q.ExamId == examId)
        //        .AsNoTracking()
        //        .ToListAsync();

        //    // 5️⃣ جلب إجابات الطالب المسجلة باستخدام (QuestionOptionId) 🎯
        //    var studentAnswers = await _context.StudentAnswers
        //        .Where(sa => sa.StudentExamId == submission.Id)
        //        .AsNoTracking()
        //        .ToDictionaryAsync(sa => sa.QuestionId, sa => sa.QuestionOptionId);

        //    // 6️⃣ بناء ViewModel العرض
        //    var reviewModel = new ExamReviewViewModel
        //    {
        //        ExamId = exam.Id,
        //        ExamTitle = exam.Title,
        //        IsSubmitted = hasSubmitted,
        //        IsExamEnded = isExamEnded,
        //        Questions = questions.Select(q =>
        //        {
        //            var correctOption = q.Options.FirstOrDefault(o => o.IsCorrect);

        //            return new QuestionReviewViewModel
        //            {
        //                QuestionId = q.Id,
        //                QuestionText = q.QuestionText,
        //                ImageUrl = q.ImageUrl,
        //                Mark = q.Mark,
        //                // إجابة الطالب المحددة (إن وجِدت)
        //                SelectedOptionId = studentAnswers.ContainsKey(q.Id) ? studentAnswers[q.Id] : null,
        //                // رقم الإجابة الصحيحة
        //                CorrectOptionId = correctOption != null ? correctOption.Id : 0,
        //                Options = q.Options.Select(o => new OptionReviewViewModel
        //                {
        //                    OptionId = o.Id,
        //                    OptionText = o.OptionText,
        //                    IsCorrect = o.IsCorrect
        //                }).ToList()
        //            };
        //        }).ToList()
        //    };

        //    return View(reviewModel);
        //}
        [HttpGet]
        public async Task<IActionResult> Review(int examId, int? studentId)
        {
            int targetStudentId = 0;

            // 1️⃣ التحقق مما إذا كان الطلب قادماً من الأدمن/المعلم ومرفق معه studentId
            if (studentId.HasValue && (User.IsInRole("Admin") || User.IsInRole("Teacher")))
            {
                targetStudentId = studentId.Value;
            }
            else
            {
                // 2️⃣ إذا كان الطلب قادماً من طالب
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                    return RedirectToAction("login2", "Account");

                var student = await _studentService.GetByUserIdAsync(userId);
                if (student == null) return Unauthorized();

                targetStudentId = student.Id;
            }

            // 3️⃣ جلب الامتحان والتحقق من وجوده
            var exam = await _context.Exams.FirstOrDefaultAsync(e => e.Id == examId);
            if (exam == null) return NotFound();

            // 4️⃣ جلب محاولة الطالب الأخيرة المعتمدة
            var submission = await _context.StudentExams
                .Where(se => se.ExamId == examId && se.StudentId == targetStudentId)
                .OrderByDescending(se => se.StartedAt)
                .FirstOrDefaultAsync();

            // إذا لم تكن هناك محاولة مسجلة لهذا الطالب
            if (submission == null)
            {
                TempData["ErrorMessage"] = "لا توجد محاولة مسجلة لهذا الطالب في هذا الامتحان.";
                return User.IsInRole("Admin") || User.IsInRole("Teacher")
                    ? RedirectToAction("Index", "Home")
                    : RedirectToAction("Exams", "Student");
            }

            // الشروط الخاصة بالطلاب (تُتجاوز تماماً إذا كان المستخدم أدمن أو معلم)
            if (!User.IsInRole("Admin") && !User.IsInRole("Teacher"))
            {
                bool hasSubmitted = submission.IsSubmitted;
                bool isExamEnded = DateTime.Now >= exam.EndExamTime;

                if (!hasSubmitted || !isExamEnded)
                {
                    TempData["ErrorMessage"] = "لا يمكنك عرض نموذج الإجابة إلا بعد تسليم الامتحان وانتهاء الموعد الرسمي المخصص للامتحان بالكامل.";
                    return RedirectToAction("Exams", "Student");
                }
            }

            // 5️⃣ جلب الأسئلة مع الاختيارات
            var questions = await _context.Questions
                .Include(q => q.Options)
                .Where(q => q.ExamId == examId)
                .AsNoTracking()
                .ToListAsync();

            // 6️⃣ جلب إجابات الطالب المسجلة
            var studentAnswers = await _context.StudentAnswers
                .Where(sa => sa.StudentExamId == submission.Id)
                .AsNoTracking()
                .ToDictionaryAsync(sa => sa.QuestionId, sa => sa.QuestionOptionId);

            // 7️⃣ بناء ViewModel العرض
            var reviewModel = new ExamReviewViewModel
            {
                ExamId = exam.Id,
                ExamTitle = exam.Title,
                IsSubmitted = submission.IsSubmitted,
                IsExamEnded = DateTime.Now >= exam.EndExamTime,
                Questions = questions.Select(q =>
                {
                    var correctOption = q.Options.FirstOrDefault(o => o.IsCorrect);

                    return new QuestionReviewViewModel
                    {
                        QuestionId = q.Id,
                        QuestionText = q.QuestionText,
                        ImageUrl = q.ImageUrl,
                        Mark = q.Mark,
                        // إجابة الطالب المحددة
                        SelectedOptionId = studentAnswers.ContainsKey(q.Id) ? studentAnswers[q.Id] : null,
                        // رقم الإجابة الصحيحة
                        CorrectOptionId = correctOption != null ? correctOption.Id : 0,
                        Options = q.Options.Select(o => new OptionReviewViewModel
                        {
                            OptionId = o.Id,
                            OptionText = o.OptionText,
                            IsCorrect = o.IsCorrect
                        }).ToList()
                    };
                }).ToList()
            };

            return View(reviewModel);
        }
        [HttpPost]
        [Authorize(Roles = "Admin,Teacher")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetStudentAttempt(int examId, int studentId)
        {
            // 1️⃣ البحث عن محاولات الطالب في هذا الامتحان
            var studentExam = await _context.StudentExams
                .FirstOrDefaultAsync(se => se.ExamId == examId && se.StudentId == studentId);

            if (studentExam != null)
            {
                // 2️⃣ حذف إجابات الطالب المتعلقة بهذه المحاولة من جدول الإجابات أولاً
                var answers = _context.StudentAnswers.Where(sa => sa.StudentExamId == studentExam.Id);
                _context.StudentAnswers.RemoveRange(answers);

                // 3️⃣ حذف سجل المحاولة نفسه
                _context.StudentExams.Remove(studentExam);

                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "تمت إعادة فتح المحاولة للطالب بنجاح!" });
            }

            return Json(new { success = false, message = "لم يتم العثور على محاولة مسجلة لهذا الطالب." });
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