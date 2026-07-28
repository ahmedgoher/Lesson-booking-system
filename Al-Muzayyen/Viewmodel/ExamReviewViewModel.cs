namespace Al_Muzayyen.Viewmodel
{
    public class ExamReviewViewModel
    {
        public int ExamId { get; set; }
        public string ExamTitle { get; set; }
        public bool IsSubmitted { get; set; }
        public bool IsExamEnded { get; set; }
        public List<QuestionReviewViewModel> Questions { get; set; } = new();
    }

    public class QuestionReviewViewModel
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public string ImageUrl { get; set; }
        public int Mark { get; set; }
        public int? SelectedOptionId { get; set; } // إجابة الطالب (إن وجدت)
        public int CorrectOptionId { get; set; }  // الإجابة الصحيحة
        public List<OptionReviewViewModel> Options { get; set; } = new();
    }

    public class OptionReviewViewModel
    {
        public int OptionId { get; set; }
        public string OptionText { get; set; }
        public bool IsCorrect { get; set; }
    }
}
