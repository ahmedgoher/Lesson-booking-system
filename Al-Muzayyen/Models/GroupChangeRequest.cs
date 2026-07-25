using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Al_Muzayyen.Models
{
    public enum RequestStatus
    {
        [Display(Name ="تحت الانتظار")]
        Pending,
        [Display(Name = "مقبول")]
        Approved,
        [Display(Name = "مرفوض")]
        Rejected
    }
    public class GroupChangeRequest
    {
        [Key]
        public int Id { get; set; }

        public bool IsDismissed { get; set; }
        public int StudentId { get; set; }
        [ForeignKey("StudentId")]
        public Student? Student { get; set; }
        [Required]
        [Display(Name ="المجموعه المطلوبه")]
        public int RequestSlotId { get; set; }
        [ForeignKey("RequestSlotId")]
        public Available_slot? RequestedSlot { get; set; }

        [Display(Name = "سبب التحويل")]
        public string? Reason { get; set; }

        [Display(Name = "تاريخ الطلب")]
        public DateTime RequestDate { get; set; } = DateTime.Now;

        [Display(Name = "حالة الطلب")]
        public RequestStatus Status { get; set; } = RequestStatus.Pending;
    }
}
