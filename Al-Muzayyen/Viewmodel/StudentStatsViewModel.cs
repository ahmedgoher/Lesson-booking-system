namespace Al_Muzayyen.Viewmodel
{
    public class StudentStatsViewModel
    {
        public string AttendanceRate { get; set; } = "0%";
        public string AttendanceDetails { get; set; } = string.Empty;
        public int AbsentCount { get; set; }
        public int CompletedExams { get; set; }
        public string AvgScore { get; set; } = "0%";
    }
}
