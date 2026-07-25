namespace Al_Muzayyen.Viewmodel
{
    public class StudentExamPageViewModel
    {
        public int ExamId { get; set; }

        public string Title { get; set; } = "";

        public int DurationMinutes { get; set; }

        public List<StudentQuestionViewModel> Questions { get; set; } = new();
    }
}