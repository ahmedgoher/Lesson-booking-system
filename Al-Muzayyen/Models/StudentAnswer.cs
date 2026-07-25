using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Al_Muzayyen.Models
{
    public class StudentAnswer
    {
        [Key]
        public int Id { get; set; }

        // محاولة الامتحان
        public int StudentExamId { get; set; }
        public StudentExam StudentExam { get; set; }

        // السؤال
        public int QuestionId { get; set; }
        public Question Question { get; set; }

        // الاختيار الذى اختاره الطالب
        public int QuestionOptionId { get; set; }
        public QuestionOption QuestionOption { get; set; }

        // صح أم خطأ
        public bool IsCorrect { get; set; }

        // درجة السؤال
        public int EarnedMarks { get; set; }
    }
}