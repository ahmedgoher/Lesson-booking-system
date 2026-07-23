using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Al_Muzayyen.Models
{
    public enum MaterialType { PDF, VideoLink, DriveLink }

    public class Material
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "عنوان الملف مطلوب")]
        [Display(Name = "العنوان")]
        public string Title { get; set; }

        [Required(ErrorMessage = "الرابط مطلوب")]
        [Display(Name = "رابط الملف أو Drive")]
        public string Url { get; set; }
        [Display(Name = "الوصف")]
        public string? Description { get; set; }

        [Display(Name = "نوع المادة")]              
        public MaterialType Type { get; set; } = MaterialType.PDF;

        [DataType(DataType.Date)]
        [Display(Name = "تاريخ الإضافة")]
        public DateTime CreatedAt { get; set; } = DateTime.Today;

        [ForeignKey("AvailableSlot")]
        public int SlotId { get; set; }
        public Available_slot? AvailableSlot { get; set; }
    }
}