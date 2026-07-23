using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Al_Muzayyen.Models
{
    public enum AttendanceStatus
    {
        [Display(Name = "حاضر")]
        Present,

        [Display(Name = "غائب")]
        Absent,

        [Display(Name = "معذور / إجازة")]
        Excused
    }
    public enum HomeworkStatus
    {
        [Display(Name = "تم / سلم الواجب")]
        Done,

        [Display(Name = "لم يسلم")]
        NotDone,

        [Display(Name = "لا يوجد واجب")]
        NoHomework
    }

    public class Attendance
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "تاريخ اليوم")]
        public DateTime Date { get; set; } = DateTime.Today;

        [Display(Name = "حالة الحضور")]
        public AttendanceStatus IsPresent { get; set; } = AttendanceStatus.Present;

        [Display(Name = "حالة الواجب")]
        public HomeworkStatus Homework { get; set; } = HomeworkStatus.Done;

        [Display(Name = "ملاحظات على الطالب")]
        public string? Notes { get; set; }

        // --- العلاقات ---
        [ForeignKey("Student")]
        public int StudentId { get; set; }
        public Student? Student { get; set; }

        [ForeignKey("AvailableSlot")]
        public int SlotId { get; set; }
        public Available_slot? AvailableSlot { get; set; }
    }
}