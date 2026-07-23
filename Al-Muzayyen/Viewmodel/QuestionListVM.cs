using Al_Muzayyen.Models;

namespace Al_Muzayyen.Viewmodel
{
    public class QuestionListVM
    {
        public int Id { get; set; }

        public string QuestionText { get; set; }

        public string? ImageUrl { get; set; }

        public int Mark { get; set; }

        public QuestionType Type { get; set; }

        public List<QuestionOption> Options { get; set; } = new();
    }
}
