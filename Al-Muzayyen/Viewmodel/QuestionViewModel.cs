namespace Al_Muzayyen.Viewmodel
{
    public class QuestionViewModel
    {
        public int Id { get; set; }
        public int ExamId { get; set; }

        public string QuestionText { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        public int Mark { get; set; } = 1;

        public string Type { get; set; } = "MCQ";

        public string OptionA { get; set; } = string.Empty;

        public string OptionB { get; set; } = string.Empty;

        public string OptionC { get; set; } = string.Empty;

        public string OptionD { get; set; } = string.Empty;

        public string CorrectAnswer { get; set; } = "A";
    }
}