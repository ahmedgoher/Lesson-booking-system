using Al_Muzayyen.Models;

namespace Al_Muzayyen.Services
{
    public interface IStudentService
    {
        Task<Student?> GetByUserIdAsync(string userId);
    }
}
