namespace Al_Muzayyen.Viewmodel
{
    public class ExamViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int GradeId { get; set; }
        public string? GradeName { get; set; }
        public DateTime Date { get; set; }
        public int Duration { get; set; }
        public DateTime? OpenDate { get; set; }
        public DateTime? CloseDate { get; set; }
        public int TotalMarks { get; set; }
        public int PassingMarks { get; set; }
        public int MaxAttempts { get; set; }
        public string Status { get; set; } = "Active";
        public bool IsPaperExam { get; set; }

        // خيارات العشوائية والمراجعة
        public bool RandomQuestions { get; set; }
        public bool ShuffleAnswers { get; set; }
        public bool AllowReview { get; set; }
        public bool ShowResult { get; set; }

        public int QuestionsCount { get; set; }
    }
}