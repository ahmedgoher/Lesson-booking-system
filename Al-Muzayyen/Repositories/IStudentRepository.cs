using Al_Muzayyen.Models;

namespace Al_Muzayyen.Repositories
{
    public interface IStudentRepository
    {
        Task<Student?> GetByUserIdAsync(string userId);
    }
}
