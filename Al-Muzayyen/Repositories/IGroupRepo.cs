using Al_Muzayyen.Models;
using System.Text.RegularExpressions;

namespace Al_Muzayyen.Repositories
{
    public interface IGroupRepo:IGenericRepository<Available_slot>
    {
        public Task<List<Available_slot>> GetAllGroupsWithRelations();
        Task UpdateGroupWithSlots(Available_slot group);
    }
}
