using DocumentFormat.OpenXml.Office.SpreadSheetML.Y2023.MsForms;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Al_Muzayyen.Models
{
    public class Exam
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "عنوان الامتحان مطلوب")]
        [Display(Name = "عنوان الامتحان")]
        public string Title { get; set; }

        [Display(Name = "مدة الامتحان بالدقائق")]
        public int DurationMinutes { get; set; } = 30;

        [Display(Name = "حالة الامتحان")]
        public bool IsActive { get; set; } = true;

        [DataType(DataType.Date)]
        [Display(Name = "تاريخ الامتحان")]
        public DateTime CreatedAt { get; set; } = DateTime.Today;
        [DataType(DataType.Date)]
        [Display(Name = "وقت بداية الامتحان")]
        public DateTime startExamTime { get; set; } 
        [DataType(DataType.Date)]
        [Display(Name = "وقت نهاية الامتحان")]
        public DateTime endExamTime  { get; set; }

        [ForeignKey("Class")]
        public int ClassId { get; set; }
        public Class? Class { get; set; }

        public List<Question> Questions { get; set; } = new List<Question>();
        public List<StudentExam> StudentExams { get; set; } = new List<StudentExam>();
    }
}