using System.ComponentModel.DataAnnotations;

namespace Al_Muzayyen.Models
{
    public class Class
    {
        [Key] // تحديد المعرف الفريد
        public int Id { get; set; }

        [Required(ErrorMessage = "اسم الصف الدراسي أو المجموعة مطلوب")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "الاسم يجب أن يكون بين 2 إلى 100 حرف")]
        [Display(Name = "اسم الصف/المجموعة")]
        public string Name { get; set; }
    }
}