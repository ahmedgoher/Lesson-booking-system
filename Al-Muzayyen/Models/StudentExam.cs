using Al_Muzayyen.Models;
using System.ComponentModel.DataAnnotations;

public class StudentExam
{
    [Key]
    public int Id { get; set; }

    public int StudentId { get; set; }
    public Student? Student { get; set; }

    public int ExamId { get; set; }
    public Exam? Exam { get; set; }

    // بداية الامتحان
    public DateTime StartedAt { get; set; }

    // نهاية الوقت المسموح
    public DateTime EndTime { get; set; }

    // وقت التسليم
    public DateTime? SubmittedAt { get; set; }

    // الدرجة
    public int Score { get; set; }

    // هل الطالب سلم؟
    public bool IsSubmitted { get; set; }

    // رقم المحاولة
    public int AttemptNumber { get; set; } = 1;
}