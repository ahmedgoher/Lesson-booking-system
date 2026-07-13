using Al_Muzayyen.Models;
using Microsoft.EntityFrameworkCore;

namespace Al_Muzayyen.Repositories
{
    public class BookingRepo : GenericRepository<Booking>, IBookingRepo
    {
        public BookingRepo(AppDbContext context) : base(context)
        {
        }

        public async Task<List<Booking>> GetEnteredStudents()
        {
            return await _context.Bookings
                    .Include(b => b.Class)
                    .Include(b => b.Place)
                    .Include(b => b.AvailableSlot)
                    .ThenInclude(a=>a.SlotTimes)
                    .ToListAsync();
        }
    }
}
