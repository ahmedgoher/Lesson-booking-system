using System.ComponentModel.DataAnnotations;

namespace Al_Muzayyen.ViewModels
{
    public class StudentProfileVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "يرجى إدخال اسم الطالب")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "يرجى إدخال رقم الهاتف")]
        [Phone(ErrorMessage = "رقم الهاتف غير صحيح")]
        public string StdPhone { get; set; } = string.Empty;

        [Required(ErrorMessage = "يرجى إدخال رقم ولي الأمر")]
        [Phone(ErrorMessage = "رقم الهاتف غير صحيح")]
        public string ParentPhone { get; set; } = string.Empty;

        // تفاصيل العرض فقط (الشهادات والصفوف)
        public string? ClassName { get; set; }
        public string? GroupName { get; set; }

        // خاصية تغيير كلمة المرور
        public ChangePasswordVM PasswordModel { get; set; } = new ChangePasswordVM();
    }

    public class ChangePasswordVM
    {
        [Required(ErrorMessage = "كلمة المرور الحالية مطلوبة")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "كلمة المرور الجديدة مطلوبة")]
        [MinLength(6, ErrorMessage = "كلمة المرور يجب أن لا تقل عن 6 أرقام/حروف")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "تأكيد كلمة المرور مطلوب")]
        [Compare("NewPassword", ErrorMessage = "كلمتا المرور غير متطابقتين")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}