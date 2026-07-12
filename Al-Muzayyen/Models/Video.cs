using System.ComponentModel.DataAnnotations;

namespace Al_Muzayyen.Models
{
    public class Video
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "يجب إدخال عنوان الفيديو")]
        [StringLength(150, ErrorMessage = "عنوان الفيديو طويل جداً")]
        [Display(Name = "عنوان الفيديو")]
        public string Title { get; set; }

        [StringLength(500, ErrorMessage = "الوصف لا يمكن أن يتجاوز 500 حرف")]
        [Display(Name = "وصف الفيديو")]
        public string Description { get; set; } // تعديل الحرف الأول لكابيتال لتناسق الكود

        [Required(ErrorMessage = "يجب إدخال رابط الفيديو")]
        [Url(ErrorMessage = "الرابط غير صحيح، يجب أن يكون لينك صالح (مثل: https://...)")]
        [Display(Name = "رابط الفيديو (URL)")]
        public string URL { get; set; }
    }
}