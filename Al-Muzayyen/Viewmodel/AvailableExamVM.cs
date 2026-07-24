namespace Al_Muzayyen.Viewmodel
{
    public class AvailableExamVM
    {
        public int ExamId { get; set; }

        public string Title { get; set; }

        public DateTime StartExamTime { get; set; }

        public DateTime EndExamTime { get; set; }

        public bool AlreadyAdded { get; set; }
    }
}
