using Al_Muzayyen.Models;
using Al_Muzayyen.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Al_Muzayyen.Services
{
    public class SlotService : ISlotService
    {
        private readonly IGenericRepository<Available_slot> _slotRepo;
        private readonly AppDbContext _context; // حقن الـ DbContext مباشرة

        // تعديل الـ Constructor ليقبل الـ DbContext
        public SlotService(IGenericRepository<Available_slot> slotRepo, AppDbContext context)
        {
            _slotRepo = slotRepo;
            _context = context;
        }

        public async Task<IEnumerable<Available_slot>> GetSlotsByFilterAsync(int classId, int placeId)
        {
            // استخدام الـ DbContext مباشرة لعمل Include لجدول الـ SlotTimes
            return await _context.Available_Slots
                .Include(s => s.SlotTimes)
                .Where(s => s.ClassId == classId && s.PlaceId == placeId)
                .ToListAsync();
        }

        public async Task<bool> CreateSlotAsync(Available_slot newSlot)
        {
            await _slotRepo.AddAsync(newSlot);
            return await _slotRepo.SaveChangesAsync();
        }
    }
}