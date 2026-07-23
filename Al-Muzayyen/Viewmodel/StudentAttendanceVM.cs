using Al_Muzayyen.Models;

namespace Al_Muzayyen.Viewmodel
{
    public class StudentAttendanceVM
    {
        public DateTime Date { get; set; }
        public AttendanceStatus IsPresent { get; set; }
        public HomeworkStatus Homework { get; set; }
        public string? Notes { get; set; }
    }
}
