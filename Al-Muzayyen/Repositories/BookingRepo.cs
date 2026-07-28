using Al_Muzayyen.Models;
using Microsoft.EntityFrameworkCore;

namespace Al_Muzayyen.Repositories
{
    public class BookingRepo : GenericRepository<Student>, IBookingRepo
    {
        public BookingRepo(AppDbContext context) : base(context)
        {
        }

        public async Task<List<Student>> GetEnteredStudents()
        {
            return await _context.Students
                .Where(s=> s.IsActive==true)
                
                    .Include(b => b.Class)
                    .Include(b => b.Place)
                    .Include(b => b.AvailableSlot)
                    .ThenInclude(a=>a.SlotTimes)
                    .ToListAsync();
        }
    }
}
