namespace Al_Muzayyen.ViewModels
{

    public class StudentExamViewModel
    {
        public int ExamId { get; set; }

        public string ExamTitle { get; set; } = "";

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public bool IsCompleted { get; set; }

        public double? Score { get; set; }

        public double TotalMarks { get; set; }
        public bool IsPaperExam { get; set; }

        public string Status { get; set; } = "";
    }
}