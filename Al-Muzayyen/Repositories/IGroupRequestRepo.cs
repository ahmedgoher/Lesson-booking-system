using Al_Muzayyen.Models;

namespace Al_Muzayyen.Repositories
{
    public interface IGroupRequestRepo : IGenericRepository<GroupChangeRequest>
    {
        List<GroupChangeRequest> GetPendingRequestsWithDetails();
    }
}
