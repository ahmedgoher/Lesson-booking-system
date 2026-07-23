using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Al_Muzayyen.Models
{
    public class Available_slot
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "يجب إدخال اسم المجموعة")]
        [StringLength(100, ErrorMessage = "اسم المجموعة طويل جداً")]
        [Display(Name = "اسم المجموعة")]
        public string Group_Name { get; set; }

        [Required(ErrorMessage = "حالة المجموعة مطلوبة")]
        [StringLength(20)]
        [Display(Name = "الحالة")]
        public string State { get; set; } = "Active";

        // --- العلاقات ---
        [Required(ErrorMessage = "يرجى تحديد المكان")]
        [ForeignKey("Place")]
        public int PlaceId { get; set; }
        public Place? Place { get; set; }

        [Required(ErrorMessage = "يرجى تحديد الصف الدراسي")]
        [ForeignKey("Class")]
        public int ClassId { get; set; }
        public Class? Class { get; set; }

        public List<Student> Students { get; set; } = new List<Student>();
        public List<Slot_time> SlotTimes { get; set; } = new List<Slot_time>();
        public List<Attendance> Attendances { get; set; } = new List<Attendance>();
        public List<Material> Materials { get; set; } = new List<Material>();

        [Required(ErrorMessage = "يجب تحديد رقم اليوم")]
        [Range(1, 7, ErrorMessage = "رقم اليوم يجب أن يكون بين 1 (السبت مثلاً) إلى 7")]
        [Display(Name = "رقم اليوم في الأسبوع")]
        public int Number_Of_day { get; set; } // تعديل الحرف الأول لكابيتال لتناسق الكود

        

        [Required(ErrorMessage = "تاريخ البدء مطلوب")]
        [DataType(DataType.Date)]
        [Display(Name = "تاريخ البدء")]
        public DateTime StartDate { get; set; } // تعديل الحرف الأول لكابيتال
        public List<ExamGroup> ExamGroups { get; set; } = new List<ExamGroup>();
    }
}