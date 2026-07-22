using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Al_Muzayyen.Models
{
    public class StudentExam
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Student")]
        public int StudentId { get; set; }
        public Student? Student { get; set; }

        [ForeignKey("Exam")]
        public int ExamId { get; set; }
        public Exam? Exam { get; set; }

        [Display(Name = "الدرجة التي حصل عليها")]
        public int Score { get; set; }

        [Display(Name = "وقت تقديم الامتحان")]
        public DateTime SubmittedAt { get; set; } = DateTime.Now;
    }
}