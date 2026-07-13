using Al_Muzayyen.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Al_Muzayyen.Repositories
{
    public class GroupRepo : GenericRepository<Available_slot>, IGroupRepo
    {
        public GroupRepo(AppDbContext context) : base(context)
        {
        }

        public async Task<List<Available_slot>> GetAllGroupsWithRelations()
        {
            return await _context.Available_Slots
                 .Include(a => a.Place)
                 .Include(a => a.Class)
                 .Include(a => a.SlotTimes)
                 .ToListAsync();
        }

        public async Task UpdateGroupWithSlots(Available_slot group)
        {
            var dbGroup = await _context.Available_Slots
                .Include(g => g.SlotTimes)
                .FirstOrDefaultAsync(g => g.Id == group.Id);

            if (dbGroup == null)
                return;

            // تحديث بيانات المجموعة
            dbGroup.Group_Name = group.Group_Name;
            dbGroup.PlaceId = group.PlaceId;
            dbGroup.ClassId = group.ClassId;
            dbGroup.Number_Of_day = group.Number_Of_day;
            dbGroup.StartDate = group.StartDate;
            dbGroup.State = group.State;

            // حذف الـ SlotTimes القديمة من قاعدة البيانات
            _context.Slot_Times.RemoveRange(dbGroup.SlotTimes);

            // إضافة الجديدة
            dbGroup.SlotTimes = group.SlotTimes.Select(x => new Slot_time
            {
                Day = x.Day,
                Time = x.Time,
                SlotID = dbGroup.Id
            }).ToList();

            await _context.SaveChangesAsync();
        }
    }
}
