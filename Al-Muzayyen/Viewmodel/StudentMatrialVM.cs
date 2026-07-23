using Al_Muzayyen.Models;

namespace Al_Muzayyen.Viewmodel
{
    public class StudentMatrialVM
    {
        public string GroupName { get; set; }
        public List<Material> Matrials { get; set; } = new List<Material>();
    }
  
}
