using Al_Muzayyen.Models;
using Microsoft.EntityFrameworkCore;

namespace Al_Muzayyen.Repositories
{
    public class GroupRequestRepo : GenericRepository<GroupChangeRequest>, IGroupRequestRepo
    {
        public GroupRequestRepo(AppDbContext context) : base(context)
        {
        }

        public List<GroupChangeRequest> GetPendingRequestsWithDetails()
        {
            return _context.GroupChangeRequests
                    .Include(r => r.Student)
                        .ThenInclude(s => s.Place)
                    .Include(r => r.Student)
                        .ThenInclude(s => s.Class)
                    .Include(r => r.RequestedSlot)
                    .Where(r => r.Status == RequestStatus.Pending)
                    .ToList();
        }
    }
}
