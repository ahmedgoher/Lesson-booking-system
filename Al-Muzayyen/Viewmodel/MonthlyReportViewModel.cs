using Al_Muzayyen.Models;

namespace Al_Muzayyen.Viewmodel
{
    public class MonthlyReportViewModel
    {
        public string StudentName { get; set; }
        public string ParentPhone { get; set; }
        public string GroupName { get; set; }
        public string GradeLevel { get; set; }
        public string CenterName { get; set; }
        public List<Slot_time> DaysSchedule { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int AttendancePercentage { get; set; }
        public string AverageExams { get; set; }
        public string HomeworkCompletion { get; set; }
        public string GeneralEvaluation { get; set; }
        public string TeacherNotes { get; set; }
        public List<AttendanceDetails> AttendanceRecord { get; set; }
        public List<ExamDetails> ExamRecord { get; set; }
    }
    public class AttendanceDetails
    {
        public DateTime Date { get; set; }
        public string LessonTitle { get; set; }
        public string Status { get; set; }
        public string HomeworkStatus { get; set; }
        public string Notes { get; set; }
    }
    public class ExamDetails
    {
        public string ExamName { get; set; }
        public DateTime ExamDate { get; set; }
        public int TotalMarks { get; set; }
        public double? StudentScore { get; set; }
        public string GradeText { get; set; } // ممتاز، جيد جداً.. إلخ
        public string Notes { get; set; }
    }
}
