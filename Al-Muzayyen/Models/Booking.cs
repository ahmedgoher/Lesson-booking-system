using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Al_Muzayyen.Models
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "يجب ادخال اسم الطالب")]
        [MaxLength(50, ErrorMessage = "لا يمكن تجاوز 50 حرف للاسم")]
        [MinLength(3, ErrorMessage = "لا يمكن للاسم ان يقل عن 3 حروف")]
        [Display(Name = "اسم الطالب")]
        public string STD_Name { get; set; }

        [Required(ErrorMessage = "رقم هاتف الطالب مطلوب")]
        [RegularExpression(@"^(010|011|012|015)\d{8}$", ErrorMessage = "رقم الهاتف غير صحيح، يجب أن يبدأ بـ 012 أو 011 أو 010 أو 015 ويتكون من 11 رقم")]
        [Display(Name = "رقم هاتف الطالب")]
        public string Student_phone { get; set; }

        [Required(ErrorMessage = "رقم هاتف ولي الأمر مطلوب")]
        [RegularExpression(@"^(010|011|012|015)\d{8}$", ErrorMessage = "رقم الهاتف غير صحيح، يجب أن يبدأ بـ 012 أو 011 أو 010 أو 015 ويتكون من 11 رقم")]
        [Display(Name = "رقم هاتف ولي الأمر")]
        public string Parent_phone { get; set; }

        // جعل التاريخ ينشأ تلقائياً في قاعدة البيانات وقت الحجز
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "ملاحظات أو شكاوى")]
        public string? Problem_text { get; set; }

        // --- العلاقات والـ Foreign Keys ---

        [Required]
        [ForeignKey("Class")]
        public int ClassId { get; set; }
        public Class? Class { get; set; } // إضافة ? لمنع مشاكل الـ Validation في الفورم

        [Required]
        [ForeignKey("AvailableSlot")]
        public int SlotId { get; set; }
        public Available_slot? AvailableSlot { get; set; } // تعديل الاسم ليبدأ بحرف كابيتال

        [Required]
        [ForeignKey("Place")]
        public int PlaceId { get; set; }
        public Place? Place { get; set; } // تعديل الاسم ليبدأ بحرف كابيتال
    }
}