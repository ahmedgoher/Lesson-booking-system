using Al_Muzayyen.Models;

namespace Al_Muzayyen.Services
{
    public interface IClassService
    {
        Task<IEnumerable<Class>> GetAllClassesAsync();
        Task<bool> CreateClassAsync(Class newClass); // هتحتاجها في صفحة الآدمن
    }
}