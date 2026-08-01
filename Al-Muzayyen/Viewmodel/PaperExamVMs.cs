using System;
using System.Collections.Generic;
using Al_Muzayyen.Models; // تأكد من استدعاء النايم سبيس الخاص بالـ AttendanceStatus

namespace Al_Muzayyen.ViewModels
{
    // 1. موديل جلب الطلاب وشاشة العرض (Response)
    public class StudentExamStatusViewModel
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;

        // حالة الحضور من جدول الحضور لليوم ده
        public AttendanceStatus Status { get; set; }

        // خاصية سهلة تعرفنا هل الطالب حاضر ولا لا لسهولة التعامل في الجافاسكربت
        public bool IsPresent => Status == AttendanceStatus.Present;

        // الدرجة المسجلة سابقاً في داتابيز الامتحانات (إن وجدت)، وستكون null إذا لم تُحفظ بعد
        public int? Score { get; set; }
    }

    // 2. موديل استقبال البيانات عند الحفظ (Request DTO)
    public class SavePaperExamScoresDto
    {
        public int SlotId { get; set; }
        public int ClassId { get; set; }
        public DateTime ExamDate { get; set; }
        public string Title { get; set; } = string.Empty;
        public int TotalMarks { get; set; }
        public int PassMark { get; set; }
        public string? Description { get; set; }

        // قائمة درجات الطلاب المدخلة
        public List<StudentScoreItemDto> Scores { get; set; } = new List<StudentScoreItemDto>();
    }

    public class StudentScoreItemDto
    {
        public int StudentId { get; set; }

        // Nullable عشان لو الطالب حاضِر بس لسه الأستاذ مدخلش درجته ونوى يكملها بعدين
        public int? Score { get; set; }

        public AttendanceStatus Status { get; set; }
    }
}