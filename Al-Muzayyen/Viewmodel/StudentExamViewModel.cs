namespace Al_Muzayyen.ViewModels
{

    public class StudentExamViewModel
    {
        public int ExamId { get; set; }

        public string ExamTitle { get; set; } = string.Empty;

        // بداية الامتحان
        public DateTime StartTime { get; set; }

        // نهاية الامتحان
        public DateTime EndTime { get; set; }

        public bool IsCompleted { get; set; }

        public double? Score { get; set; }

        public double TotalMarks { get; set; }

        // الحالة
        public string Status { get; set; } = "";
    }
}