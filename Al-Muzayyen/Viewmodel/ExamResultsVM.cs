namespace Al_Muzayyen.Viewmodel
{
    public class ExamResultsVM
    {
        public int ExamId { get; set; }

        public string ExamTitle { get; set; }

        public int PassingMarks { get; set; }

        public List<StudentExamResultVM> Students { get; set; } = new();
    }
}
