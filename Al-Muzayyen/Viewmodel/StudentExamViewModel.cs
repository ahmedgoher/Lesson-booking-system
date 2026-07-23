namespace Al_Muzayyen.ViewModels
{
    public class StudentExamViewModel
    {
        public int ExamId { get; set; }
        public string ExamTitle { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public bool IsCompleted { get; set; }
        public double? Score { get; set; }
        public double TotalMarks { get; set; }
    }
}