namespace Al_Muzayyen.Viewmodel
{
    public class StudentExamResultVM
    {
        public int StudentId { get; set; }

        public string StudentName { get; set; }

        public int? Score { get; set; }

        public string Status { get; set; }

        public DateTime? SubmittedAt { get; set; }
    }
}