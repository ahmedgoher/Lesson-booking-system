using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Al_Muzayyen.Models
{
    public class Admin
    {
        [Key]
        public int Id { get; set; }

        [StringLength(100, ErrorMessage = "الاسم طويل جداً")]
        [Display(Name = "اسم الأدمن / المدرس")]
        public string? Name { get; set; }

        [Display(Name = "رابط الصورة الشخصية")]
        [Column(TypeName = "nvarchar(max)")]
        public string? ImageUrl { get; set; }

        [RegularExpression(@"^(010|011|012|015)\d{8}$", ErrorMessage = "رقم الهاتف غير صحيح، يجب أن يتكون من 11 رقم مصري")]
        [Display(Name = "رقم التواصل")]
        public string? PhoneNumber { get; set; }

        // --- الربط مع حساب الـ Identity ---
        [ForeignKey("User")]
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
    }
}