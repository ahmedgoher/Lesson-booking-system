using Al_Muzayyen.Models;

namespace Al_Muzayyen.Viewmodel
{
    public class StudentAlertsViewModel
    {
        public List<Exam>? PendingExams { get; set; }
        public GroupChangeRequest? GroupRequest { get; set; }
    }
}
