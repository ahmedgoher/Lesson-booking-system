using System.ComponentModel.DataAnnotations;

namespace Al_Muzayyen.Viewmodel
{
    public class LoginVM
    {
        [Required(ErrorMessage = "برجاء إدخال اسم المستخدم")]
        [Display(Name = "اسم المستخدم")]
        public string Username { get; set; }

        [Required(ErrorMessage = "برجاء إدخال كلمة المرور")]
        [DataType(DataType.Password)]
        [Display(Name = "كلمة المرور")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}