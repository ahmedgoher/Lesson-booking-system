using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Al_Muzayyen.Models
{
    public class Exam
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "عنوان الامتحان مطلوب")]
        [Display(Name = "عنوان الامتحان")]
        public string Title { get; set; }

        [Display(Name = "وصف الامتحان")]
        public string? Description { get; set; }

        [Display(Name = "مدة الامتحان بالدقائق")]
        public int DurationMinutes { get; set; } = 30;

        [Display(Name = "حالة الامتحان")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "تاريخ إنشاء الامتحان")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "وقت بداية الامتحان")]
        public DateTime StartExamTime { get; set; }

        [Display(Name = "وقت نهاية الامتحان")]
        public DateTime EndExamTime { get; set; }

        // ==========================================
        // 🔹 الإعدادات والخصائص الإضافية (التي كانت ناقصة)
        // ==========================================

        [Display(Name = "الدرجة الكلية")]
        public int TotalMarks { get; set; } = 100;

        [Display(Name = "درجة النجاح")]
        public int PassingMarks { get; set; } = 50;

        [Display(Name = "عدد المحاولات المسموحة")]
        public int MaxAttempts { get; set; } = 1;

        [Display(Name = "ترتيب الأسئلة عشوائياً")]
        public bool RandomQuestions { get; set; } = false;

        [Display(Name = "ترتيب الاختيارات عشوائياً")]
        public bool ShuffleAnswers { get; set; } = false;

        [Display(Name = "السماح بمراجعة الإجابات")]
        public bool AllowReview { get; set; } = true;

        [Display(Name = "إظهار النتيجة فوراً")]
        public bool ShowResult { get; set; } = true;

        // ==========================================
        // 🔹 العلاقات (Relationships)
        // ==========================================

        [ForeignKey("Class")]
        public int ClassId { get; set; }
        public Class? Class { get; set; }

        public List<Question> Questions { get; set; } = new List<Question>();
        public List<StudentExam> StudentExams { get; set; } = new List<StudentExam>();
        public List<ExamGroup> ExamGroups { get; set; } = new List<ExamGroup>();
    }
}