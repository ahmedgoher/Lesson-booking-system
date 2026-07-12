using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Al_Muzayyen.Models
{
    public class Available_slot
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "يجب ادخال اسم المجموعة")]
        [StringLength(100, ErrorMessage = "اسم المجموعة طويل جداً")]
        [Display(Name = "اسم المجموعة")]
        public string Group_Name { get; set; }

        [Required(ErrorMessage = "يجب تحديد رقم اليوم")]
        [Range(1, 7, ErrorMessage = "رقم اليوم يجب أن يكون بين 1 (السبت مثلاً) إلى 7")]
        [Display(Name = "رقم اليوم في الأسبوع")]
        public int Number_Of_day { get; set; } // تعديل الحرف الأول لكابيتال لتناسق الكود

        [Required(ErrorMessage = "حالة الموعد مطلوبة")]
        [StringLength(20)]
        [Display(Name = "الحالة")]
        public string State { get; set; } = "Active"; // وضع قيمة افتراضية مثلاً نشط

        [Required(ErrorMessage = "تاريخ البدء مطلوب")]
        [DataType(DataType.Date)]
        [Display(Name = "تاريخ البدء")]
        public DateTime StartDate { get; set; } // تعديل الحرف الأول لكابيتال

        // --- العلاقات والـ Foreign Keys ---

        [Required]
        [ForeignKey("Place")]
        [Display(Name = "المكان")]
        public int PlaceId { get; set; } // تعديل الحرف الأول لكابيتال
        public Place? Place { get; set; } // إضافة كائن المكان الناقص

        [Required]
        [ForeignKey("Class")]
        [Display(Name = "الصف الدراسي")]
        public int ClassId { get; set; } // تعديل الحرف الأول لكابيتال
        public Class? Class { get; set; }

        // --- علاقات الـ Collections (One-to-Many) ---

        // عمل تهيئة (New) لليست لتجنب الـ Null Reference Exception
        public List<Booking> Bookings { get; set; } = new List<Booking>();
        public List<Slot_time> SlotTimes { get; set; } = new List<Slot_time>();
    }
}