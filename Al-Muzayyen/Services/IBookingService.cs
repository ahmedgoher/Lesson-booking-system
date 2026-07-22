using Al_Muzayyen.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Al_Muzayyen.Services
{
    public interface IBookingService
    {
        Task<IEnumerable<Student>> GetAllBookingsAsync();
        Task<Student?> GetBookingByIdAsync(int id);
        Task<bool> CreateBookingAsync(Student booking);
        Task<bool> DeleteBookingAsync(int id);
    }
}