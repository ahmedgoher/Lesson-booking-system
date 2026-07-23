using Al_Muzayyen.Models;

namespace Al_Muzayyen.Viewmodel
{
    public class AttendenceVM
    {
        public DateTime Date { get; set; }
        public int? classId { get; set; }
        public int? placeId { get; set; }
        public int SlotId { get; set; }
        public List<StudentAttendanceRowVM> Students { get; set; } = new();
    }
    public class StudentAttendanceRowVM
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string StudentPhone { get; set; } = string.Empty;
        public bool WasAbsentLastTime { get; set; }

        public AttendanceStatus IsPresent { get; set; } = AttendanceStatus.Present;
        public HomeworkStatus Homework { get; set; } = HomeworkStatus.Done;
        public string? Notes { get; set; }
    }
}
