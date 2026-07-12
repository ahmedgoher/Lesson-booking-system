using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Al_Muzayyen.Models
{
    public class Slot_time
    {
        [Key] // تحديد أن هذا هو المفتاح الأساسي
        public int ID { get; set; }

        [Required(ErrorMessage = "اليوم مطلوب")]
        [StringLength(20, ErrorMessage = "اسم اليوم طويل جداً")]
        public string Day { get; set; } // يُفضل تبدأ بحرف كابيتال حسب اصطلاحات C#

        [Required(ErrorMessage = "الوقت مطلوب")]
        [DataType(DataType.Time)] // عشان يظهر كـ Time Picker في الفورم لو هتدخله من لوحة التحكم
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh:mm tt}")] // عرض الوقت بصيغة AM/PM
        public DateTime Time { get; set; }

        // ربط صريح للمفتاح الأجنبي (Foreign Key)
        [Required]
        [ForeignKey("AvailableSlot")]
        public int SlotID { get; set; }

        // خاصية الملاحة (Navigation Property)
        public Available_slot AvailableSlot { get; set; }
    }
}