namespace Al_Muzayyen.Viewmodel
{
    public class GroupExamVM
    {
        public int ExamId { get; set; }

        public string Title { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public int QuestionsCount { get; set; }
    }
}