using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Al_Muzayyen.Models
{
    public class Class
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "اسم الصف الدراسي مطلوب (مثل: الصف الأول الثانوي)")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "الاسم يجب أن يكون بين 2 إلى 100 حرف")]
        [Display(Name = "الصف الدراسي")]
        public string Name { get; set; }

        public List<Available_slot> AvailableSlots { get; set; } = new List<Available_slot>();
        public List<Exam> Exams { get; set; } = new List<Exam>();
    }
}