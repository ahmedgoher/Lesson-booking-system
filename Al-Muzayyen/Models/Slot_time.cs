using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Al_Muzayyen.Models
{
    public class Slot_time
    {
        [Key]
        public int ID { get; set; }

        [Required(ErrorMessage = "اليوم مطلوب")]
        [StringLength(20)]
        [Display(Name = "اليوم")]
        public string Day { get; set; }

        [Required(ErrorMessage = "الوقت مطلوب")]
        [DataType(DataType.Time)]
        [Display(Name = "الموعد")]
        public DateTime Time { get; set; }

        [Required]
        [ForeignKey("AvailableSlot")]
        public int SlotID { get; set; }
        public Available_slot? AvailableSlot { get; set; }
    }
}