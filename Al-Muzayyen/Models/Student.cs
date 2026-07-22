using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Al_Muzayyen.Models
{
    public class Student
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "يجب إدخال اسم الطالب")]
        [MaxLength(50, ErrorMessage = "لا يمكن تجاوز 50 حرف للاسم")]
        [MinLength(3, ErrorMessage = "لا يمكن للاسم أن يقل عن 3 حروف")]
        [Display(Name = "اسم الطالب")]
        public string Name { get; set; }

        [Required(ErrorMessage = "رقم هاتف الطالب مطلوب")]
        [RegularExpression(@"^(010|011|012|015)\d{8}$", ErrorMessage = "رقم الهاتف غير صحيح، يجب أن يتكون من 11 رقم")]
        [Display(Name = "رقم هاتف الطالب")]
        public string StdPhone { get; set; }
        [Required(ErrorMessage = "رقم هاتف ولي الأمر مطلوب")]
        [RegularExpression(@"^(010|011|012|015)\d{8}$", ErrorMessage = "رقم الهاتف غير صحيح، يجب أن يتكون من 11 رقم")]
        [Display(Name = "رقم هاتف ولي الأمر")]
        public string ParentPhone { get; set; }
        [Required(ErrorMessage = "برجاء إدخال كلمة المرور")]
        [MinLength(6, ErrorMessage = "كلمة المرور يجب ألا تقل عن 6 أحرف/أرقام")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        // توكين فريد لرابط ولي الأمر عبر الواتساب بدون تسجيل دخول
        public string ParentAccessToken { get; set; } = Guid.NewGuid().ToString("N");

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }

        // --- الربط مع جدول الـ Identity User ---
        [ForeignKey("User")]
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }

        // --- العلاقات ---
        [ForeignKey("Class")]

        [Required(ErrorMessage = "يرجى اختيار الصف الدراسي")]

        [Range(1, int.MaxValue, ErrorMessage = "يرجى اختيار الصف الدراسي من القائمة")]

        public int ClassId { get; set; }

        public Class? Class { get; set; } // إضافة ? لمنع مشاكل الـ Validation
        [ForeignKey("Place")]

        public int PlaceId { get; set; }

        public Place? Place { get; set; } // تعديل الاسم ليبدأ بحرف كابيتال
        [Required(ErrorMessage = "يرجى اختيار المجموعة")]
        [ForeignKey("AvailableSlot")]
        public int SlotId { get; set; }
        public Available_slot? AvailableSlot { get; set; }

        public List<Attendance> Attendances { get; set; } = new List<Attendance>();
        public List<StudentExam> StudentExams { get; set; } = new List<StudentExam>();
    }
}