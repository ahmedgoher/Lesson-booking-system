namespace Al_Muzayyen.Viewmodel
{
    public class StudentQuestionViewModel
    {
        public int Id { get; set; }

        public string QuestionText { get; set; }

        public string? ImageUrl { get; set; }

        public string Type { get; set; }

        public List<StudentOptionViewModel> Options { get; set; } = new();
    }

    public class StudentOptionViewModel
    {
        public int Id { get; set; }

        public string OptionText { get; set; }
    }
}