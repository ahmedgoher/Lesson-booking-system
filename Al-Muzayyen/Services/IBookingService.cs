using Al_Muzayyen.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Al_Muzayyen.Services
{
    public interface IBookingService
    {
        Task<IEnumerable<Booking>> GetAllBookingsAsync();
        Task<Booking?> GetBookingByIdAsync(int id);
        Task<bool> CreateBookingAsync(Booking booking);
        Task<bool> DeleteBookingAsync(int id);
    }
}