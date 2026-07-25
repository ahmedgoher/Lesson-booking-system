using Al_Muzayyen.Models;

namespace Al_Muzayyen.Viewmodel
{
    public class StudentDashboardViewModel
    {
        public string StudentName { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public string GroupName { get; set; } = string.Empty;
        public List<SelectListItem2> AvailableSlots { get; set; } = new();
    }

  

 

    public class SelectListItem2
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
