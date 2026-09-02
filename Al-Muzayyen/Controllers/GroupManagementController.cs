using Al_Muzayyen.Models;
using Al_Muzayyen.Viewmodel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Al_Muzayyen.Controllers
{
    [Authorize(Roles = "Admin")]

    public class GroupManagementController : Controller
    {
        private readonly AppDbContext _context;

        public GroupManagementController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult GroupManagement(int id)
        {
            var group = _context.Available_Slots
                 .Include(s => s.Class)
                 .Include(s => s.Place)
                 .Include(s => s.Students)
                 .FirstOrDefault(s => s.Id == id);
            if (group == null)
            {
                return NotFound();
            }
            var allMaterials = _context.Materials
                .Where(m => m.SlotId == id)
                .OrderByDescending(m => m.CreatedAt)
                .ToList();
            var exams = _context.ExamGroup
    .Where(x => x.SlotId == id)
    .Select(x => new GroupExamVM
    {
        ExamId = x.Exam.Id,
        Title = x.Exam.Title,
        CreatedAt = x.Exam.CreatedAt,
        QuestionsCount = x.Exam.Questions.Count()
    })
    .ToList();
            // جميع الامتحانات الخاصة بنفس الصف
            var availableExams = _context.Exams
                .Where(e => e.ClassId == group.ClassId)
                .Select(e => new AvailableExamVM
                {
                    ExamId = e.Id,
                    Title = e.Title,
                    StartExamTime = e.StartExamTime,
                    EndExamTime = e.EndExamTime,

                    AlreadyAdded = _context.ExamGroup
                        .Any(g => g.SlotId == id && g.ExamId == e.Id)
                })
                .OrderByDescending(e => e.StartExamTime)
                .ToList();
            var viewModel = new GroupManagementViewModel
            {
                SlotId = group.Id,
                // تركيب الاسم باستخدام Properties الموجودة فعلياً
                GroupName = $"{group.Group_Name} ({(group.Class != null ? group.Class.Name : "")} - {(group.Place != null ? group.Place.Name : "")})",
                StudentCount = group.Students.Where(s=> s.IsActive==true).Count(),
                VideoCount = allMaterials.Count(m => m.Type == MaterialType.VideoLink),
                //ExamCount = _context.Exams != null ? _context.Exams.Count(e => e.Id == id) : 0
                ExamCount = exams.Count
                ,
                Exams = exams,
                AvailableExams = availableExams,
                Materials = allMaterials.Where(m => m.Type != MaterialType.VideoLink).ToList(),
                Videos = allMaterials.Where(m => m.Type == MaterialType.VideoLink).ToList()
            };
            return View(viewModel);
        }
        [HttpPost]
        public IActionResult AddMaterial(int slotId, string title, string url)
        {
            if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(url))
            {
                var material = new Material
                {
                    SlotId = slotId,
                    Title = title,
                    Url = url,
                    Type = MaterialType.PDF,
                    CreatedAt = DateTime.Today
                };

                _context.Materials.Add(material);
                _context.SaveChanges();
            }
            return RedirectToAction("GroupManagement", new { id = slotId });
        }
        [HttpPost]
        public IActionResult DeleteMaterial(int id, int slotId)
        {
            var item = _context.Materials.Find(id);
            if (item != null)
            {
                _context.Materials.Remove(item);
                _context.SaveChanges();
            }
            return RedirectToAction("GroupManagement", new { id = slotId });
        }

        //[HttpGet]
        //public async Task<IActionResult> GetExamResults(int examId, int slotId)
        //{
        //    var exam = await _context.Exams
        //        .FirstOrDefaultAsync(e => e.Id == examId);

        //    if (exam == null)
        //    {
        //        return Json(new
        //        {
        //            success = false,
        //            message = "الامتحان غير موجود"
        //        });
        //    }

        //    var now = DateTime.Now;

        //    var students = await _context.Students
        //        .Where(s => s.SlotId == slotId)
        //        .Select(s => new
        //        {
        //            Student = s,
        //            StudentExam = s.StudentExams
        //                .FirstOrDefault(se => se.ExamId == examId)
        //        })
        //        .ToListAsync();

        //    var result = students.Select(x =>
        //    {
        //        string status;

        //        if (x.StudentExam != null)
        //        {
        //            status = x.StudentExam.Score >= exam.PassingMarks
        //                ? "ناجح"
        //                : "راسب";
        //        }
        //        else if (now < exam.StartExamTime)
        //        {
        //            status = "لم يبدأ";
        //        }
        //        else if (now >= exam.StartExamTime && now <= exam.EndExamTime)
        //        {
        //            status = "لم يمتحن بعد";
        //        }
        //        else
        //        {
        //            status = "غائب";
        //        }

        //        return new
        //        {
        //            studentName = x.Student.Name,
        //            score = x.StudentExam?.Score,
        //            submittedAt = x.StudentExam?.SubmittedAt,
        //            status = status
        //        };
        //    });

        //    return Json(new
        //    {
        //        success = true,
        //        data = result
        //    });
        //}
        [HttpGet]
        public async Task<IActionResult> GetExamResults(int examId, int slotId)
        {
            var exam = await _context.Exams
                .FirstOrDefaultAsync(e => e.Id == examId);

            if (exam == null)
            {
                return Json(new
                {
                    success = false,
                    message = "الامتحان غير موجود"
                });
            }

            // توقيت مصر
            var egyptTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Africa/Cairo");

            var now = TimeZoneInfo.ConvertTimeFromUtc(
                DateTime.UtcNow,
                egyptTimeZone
            );

            // 🎯 هل انتهى وقت الامتحان كلياً بالنسبة للجدول الزمني؟
            bool isExamEnded = now > exam.EndExamTime;

            var students = await _context.Students
                .Where(s => s.SlotId == slotId && s.IsActive== true)
                .Select(s => new
                {
                    Student = s,
                    StudentExam = s.StudentExams
                        .FirstOrDefault(se => se.ExamId == examId)
                })
                .ToListAsync();

            var result = students.Select(x =>
            {
                string status;

                if (x.StudentExam != null)
                {
                    status = x.StudentExam.Score >= exam.PassingMarks
                        ? "ناجح"
                        : "راسب";
                }
                else if (now < exam.StartExamTime)
                {
                    status = "لم يبدأ";
                }
                else if (now >= exam.StartExamTime && now <= exam.EndExamTime)
                {
                    status = "لم يمتحن بعد";
                }
                else
                {
                    status = "غائب";
                }

                return new
                {
                    studentId = x.Student.Id, // 🎯 تم إضافة ID الطالب للتحكم
                    studentName = x.Student.Name,
                    score = x.StudentExam?.Score,
                    submittedAt = x.StudentExam?.SubmittedAt,
                    status = status
                };
            });

            return Json(new
            {
                success = true,
                isExamEnded = isExamEnded, // 🎯 تم إرسال حالة انتهاء موعد الامتحان
                data = result
            });
        }
        // إضافة فيديو جديد
        [HttpPost]
        public IActionResult AddVideo(int slotId, string title, string url)
        {
            if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(url))
            {
                string embedUrl = url;

                // تحويل رابط YouTube العادي (watch?v=xxx)
                if (url.Contains("watch?v="))
                {
                    var videoId = url.Split("watch?v=")[1].Split('&')[0];
                    embedUrl = $"https://www.youtube.com/embed/{videoId}";
                }
                // تحويل رابط YouTube المختصر (youtu.be/xxx)
                else if (url.Contains("youtu.be/"))
                {
                    var videoId = url.Split("youtu.be/")[1].Split('?')[0];
                    embedUrl = $"https://www.youtube.com/embed/{videoId}";
                }

                var video = new Material
                {
                    SlotId = slotId,
                    Title = title,
                    Url = embedUrl,
                    Type = MaterialType.VideoLink,
                    CreatedAt = DateTime.Today
                };

                _context.Materials.Add(video);
                _context.SaveChanges();
            }

            return RedirectToAction("GroupManagement", new { id = slotId });
        }
        [HttpPost]
        public IActionResult AddExamToGroup(int slotId, int examId)
        {
            bool exists = _context.ExamGroup
                .Any(x => x.SlotId == slotId && x.ExamId == examId);

            if (!exists)
            {
                _context.ExamGroup.Add(new ExamGroup
                {
                    SlotId = slotId,
                    ExamId = examId
                });

                _context.SaveChanges();
            }

            return RedirectToAction(nameof(GroupManagement), new { id = slotId });
        }

        [HttpGet]
        public async Task<IActionResult> GetGroupStudents(int slotId)
        {
            try
            {
                // 🎯 جلب الرابط الأساسي للسيرفر ديناميكياً (Domain / Host)
                var baseUrl = $"{Request.Scheme}://{Request.Host}";

                var student = await _context.Students
                    .Where(s => s.SlotId == slotId && s.IsActive == true)
                    .Select(s => new
                    {
                        studentName = s.Name,
                        ParentPhone = s.ParentPhone,
                        studentPhone = s.StdPhone,
                        studentToken = s.ParentAccessToken,

                        // 🎯 إنشاء رابط التقرير بدون m و y حتى يقرأ الشهر الحالي تلقائياً دائماً
                        reportUrl = $"{baseUrl}/Report/View?token={s.ParentAccessToken}"
                    }).ToListAsync();

                return Json(new { success = true, data = student });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "حدث خطأ أثناء جلب البيانات من السيرفر." });
            }
        }

    }
}
