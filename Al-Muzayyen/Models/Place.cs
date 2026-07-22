using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Al_Muzayyen.Models
{
    public class Place
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "يجب إدخال اسم المكان (مثل: منوف / السادات)")]
        [StringLength(150, MinimumLength = 2, ErrorMessage = "اسم المكان يجب أن يكون بين 2 إلى 150 حرف")]
        [Display(Name = "اسم المكان / السنتر")]
        public string Name { get; set; }

        [Display(Name = "عنوان السنتر")]
        public string? Address { get; set; }

        public List<Available_slot> AvailableSlots { get; set; } = new List<Available_slot>();
    }
}