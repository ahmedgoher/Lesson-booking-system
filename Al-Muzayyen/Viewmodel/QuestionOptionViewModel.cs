namespace Al_Muzayyen.Viewmodel
{
    public class QuestionOptionViewModel
    {
        public int Id { get; set; }

        public string OptionText { get; set; } = string.Empty;

        public bool IsCorrect { get; set; }
    }
}