using Al_Muzayyen.Models;

namespace Al_Muzayyen.Services
{
    public interface ISlotService
    {
        Task<IEnumerable<Available_slot>> GetSlotsByFilterAsync(int classId, int placeId);
        Task<bool> CreateSlotAsync(Available_slot newSlot); // للآدمن
    }
}