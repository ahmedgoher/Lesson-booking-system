using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Al_Muzayyen.Models
{
    public enum QuestionType { MCQ, TrueFalse }

    public class Question
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "نص السؤال مطلوب")]
        [Display(Name = "نص السؤال")]
        public string QuestionText { get; set; }

        [Display(Name = "رابط صورة السؤال (اختياري)")]
        public string? ImageUrl { get; set; }

        [Display(Name = "درجة السؤال")]
        public int Mark { get; set; } = 1;

        [Display(Name = "نوع السؤال")]
        public QuestionType Type { get; set; } = QuestionType.MCQ;

        [ForeignKey("Exam")]
        public int ExamId { get; set; }
        public Exam? Exam { get; set; }

        public List<QuestionOption> Options { get; set; } = new List<QuestionOption>();
    }
}