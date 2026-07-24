using Al_Muzayyen.Models;

namespace Al_Muzayyen.Viewmodel
{
    public class GroupManagementViewModel
    {
        public int SlotId { get; set; }
        public string GroupName { get; set; }
        public int StudentCount { get; set; }
        public int VideoCount { get; set; }
        public int ExamCount { get; set; }

        public List<Material> Materials { get; set; } = new List<Material>();
        public List<Material> Videos { get; set; } = new List<Material>();
        public List<GroupExamVM> Exams { get; set; } = new();

    }
}
