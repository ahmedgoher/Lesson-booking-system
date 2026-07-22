using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Al_Muzayyen.Models
{
    public class QuestionOption
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string OptionText { get; set; }

        [Display(Name = "إجابة صحيحة؟")]
        public bool IsCorrect { get; set; } = false;

        [ForeignKey("Question")]
        public int QuestionId { get; set; }
        public Question? Question { get; set; }
    }
}