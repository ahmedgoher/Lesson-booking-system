using Al_Muzayyen.Models;
//using Al_Muzayyen.Viewmodels;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Security.Principal;

namespace Al_Muzayyen.Viewmodel

{

        public class IndexVMAdmin
        {
            public int StudentsCount { get; set; } = 0;
            public int GroupsCount { get; set; } = 0;
            public int ClassesCount { get; set; } = 0;
            public Admin? Admin { get; set; }
        }

        // DTOs لجلب البيانات بسهولة وبسرعة عبر JS
        public class PendingBookingDTO
        {
            public int Id { get; set; }
            public string StudentName { get; set; }
            public string StdPhone { get; set; }
            public string ParentPhone { get; set; }
            public string PlaceName { get; set; }
            public string ClassName { get; set; }
            public string GroupName { get; set; }
            public string CreatedAt { get; set; }
        }

        public class GroupChangeRequestDTO
        {
            public int Id { get; set; }
            public string StudentName { get; set; }
            public string PlaceName { get; set; }
            public string ClassName { get; set; }
            public string RequestedGroup { get; set; }
            public string Reason { get; set; }
            public string RequestDate { get; set; }
            public string Status { get; set; }
        }
    }
  
