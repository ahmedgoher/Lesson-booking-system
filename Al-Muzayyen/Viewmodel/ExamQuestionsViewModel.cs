namespace Al_Muzayyen.Viewmodel
{
    public class ExamQuestionsViewModel
    {
        public int ExamId { get; set; }

        public string ExamTitle { get; set; } = string.Empty;

        public int TotalQuestions { get; set; }

        public int TotalMarks { get; set; }

        public List<QuestionViewModel> Questions { get; set; } = new();
    }
}