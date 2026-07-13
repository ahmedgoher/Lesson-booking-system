using Al_Muzayyen.Models;

namespace Al_Muzayyen.Viewmodel
{
    public class HomeVM
    {
        public List<Video>? LinkesVideos { get; set; }
        
        public string? linkimage { get; set; }

        public List<Class>? classes { get; set; }

        public List<Place>? places { get; set; }

    }


    public class videovm
    {
        public int? id { get; set; }
        public string? title { get; set; }
        public string? url { get; set; }
        public string? Description { get; set; }
    }

    public class Classvm
    {
        public int? id { get; set; }
        public string? name { get; set; }
    
    }
    public class Placevm
    {
        public int? id { get; set; }
        public string? name { get; set; }

    }

}
