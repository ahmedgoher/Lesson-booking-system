using System.ComponentModel.DataAnnotations;

namespace Al_Muzayyen.Viewmodel
{
    public class StudentLoginVM
    {
        [Required(ErrorMessage = "برجاء إدخال رقم الهاتف")]
        [RegularExpression(@"^01[0125][0-9]{8}$", ErrorMessage = "يرجى إدخال رقم هاتف مصري صحيح")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "برجاء إدخال كلمة السر")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}