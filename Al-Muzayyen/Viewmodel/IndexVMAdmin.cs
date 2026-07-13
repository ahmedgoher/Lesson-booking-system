using Al_Muzayyen.Models;

namespace Al_Muzayyen.Viewmodel

{
    public class IndexVMAdmin
    {

        public int StudentsCount { get; set; } = 0;
        public int GroupsCount { get; set; } = 0;
        public int ClassesCount { get; set; } = 0;
        public Admin? Admin { get; set; }

    }
}
