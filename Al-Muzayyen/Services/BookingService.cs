using Al_Muzayyen.Models;
using Al_Muzayyen.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Al_Muzayyen.Services
{
    public class BookingService : IBookingService
    {
        private readonly IGenericRepository<Booking> _bookingRepo;

        public BookingService(IGenericRepository<Booking> bookingRepo)
        {
            _bookingRepo = bookingRepo;
        }

        // جلب كل الحجوزات مع بيانات المكان والصف والموعد للآدمين
        public async Task<IEnumerable<Booking>> GetAllBookingsAsync()
        {
            return await _bookingRepo.GetAllAsync();
            // ملحوظة: لو حابب تعمل Include للتفاصيل، يفضل كتابة كود مخصص في الـ Repo
            // لكن كبداية GetAllAsync تفي بالغرض جداً.
        }

        public async Task<Booking?> GetBookingByIdAsync(int id) => await _bookingRepo.GetByIdAsync(id);

        public async Task<bool> CreateBookingAsync(Booking booking)
        {
            await _bookingRepo.AddAsync(booking);
            return await _bookingRepo.SaveChangesAsync();
        }

        public async Task<bool> DeleteBookingAsync(int id)
        {
            var booking = await _bookingRepo.GetByIdAsync(id);
            if (booking == null) return false;

            _bookingRepo.Delete(booking);
            return await _bookingRepo.SaveChangesAsync();
        }
    }
}