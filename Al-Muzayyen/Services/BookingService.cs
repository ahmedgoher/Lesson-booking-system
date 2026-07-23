using Al_Muzayyen.Models;
using Al_Muzayyen.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq; // 👈 إضافة System.Linq لاستخدام Any
using System.Threading.Tasks;

namespace Al_Muzayyen.Services
{
    public class BookingService : IBookingService
    {
        private readonly IGenericRepository<Student> _bookingRepo;

        public BookingService(IGenericRepository<Student> bookingRepo)
        {
            _bookingRepo = bookingRepo;
        }

        // جلب كل الحجوزات مع بيانات المكان والصف والموعد للآدمين
        public async Task<IEnumerable<Student>> GetAllBookingsAsync()
        {
            return await _bookingRepo.GetAllAsync();
        }

        public async Task<Student?> GetBookingByIdAsync(int id) => await _bookingRepo.GetByIdAsync(id);

        public async Task<bool> CreateBookingAsync(Student booking)
        {
            // 🟢 1. التحقق من عدم وجود رقم الهاتف مسبقاً
            var allStudents = await _bookingRepo.GetAllAsync();
            bool phoneExists = allStudents.Any(s => s.StdPhone == booking.StdPhone);

            if (phoneExists)
            {
                return false; // إرجاع false ليقوم الـ Controller بإظهار رسالة الخطأ المناسبة
            }

            // 🟢 2. إضافة الطالب في حال كان الرقم غير مكرر
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