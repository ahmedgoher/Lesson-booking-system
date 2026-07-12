using System.ComponentModel.DataAnnotations;

namespace Al_Muzayyen.Models
{
    public class Admin
    {
        [Key]
        public int Id { get; set; }

        [StringLength(100, ErrorMessage = "الاسم طويل جداً")]
        [Display(Name = "اسم الأدمن / المدرس")]
        public string? Name { get; set; }

        [StringLength(500)]
        [Display(Name = "رابط الصورة الشخصية")]
        public string? ImageUrl { get; set; }

        [RegularExpression(@"^(010|011|012|015)\d{8}$", ErrorMessage = "رقم الهاتف غير صحيح، يجب أن يتكون من 11 رقم مصري")]
        [Display(Name = "رقم التواصل")]
        public string? PhoneNumber { get; set; } // تعديل الحرف الأول لكابيتال لتناسق الكود
    }
}