using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Al_Muzayyen.Models
{
    public class Place
    {
        [Key] // تحديد المفتاح الأساسي
        public int Id { get; set; }

        [Required(ErrorMessage = "يجب إدخال اسم المكان أو السنتر")]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "اسم المكان يجب أن يكون بين 3 إلى 150 حرف")]
        [Display(Name = "اسم المكان/السنتر")]
        public string Name { get; set; }

        // --- علاقة عكسية (Navigation Property) ---
        // بما إن المكان الواحد ممكن يكون فيه أكتر من مجموعة/موعد متاح
        public List<Available_slot> AvailableSlots { get; set; } = new List<Available_slot>();
    }
}