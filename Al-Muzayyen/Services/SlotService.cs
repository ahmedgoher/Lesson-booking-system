using Al_Muzayyen.Models;
using Al_Muzayyen.Repositories;

namespace Al_Muzayyen.Services
{
    public class SlotService : ISlotService
    {
        private readonly IGenericRepository<Available_slot> _slotRepo;
        public SlotService(IGenericRepository<Available_slot> slotRepo) => _slotRepo = slotRepo;

        public async Task<IEnumerable<Available_slot>> GetSlotsByFilterAsync(int classId, int placeId)
        {
            var allSlots = await _slotRepo.GetAllAsync();
            return allSlots.Where(s => s.ClassId == classId && s.PlaceId == placeId).ToList();
        }
        public async Task<bool> CreateSlotAsync(Available_slot newSlot) { await _slotRepo.AddAsync(newSlot); return await _slotRepo.SaveChangesAsync(); }
    }
}
