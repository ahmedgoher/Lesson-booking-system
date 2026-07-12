using Al_Muzayyen.Models;
using Al_Muzayyen.Repositories;

namespace Al_Muzayyen.Services
{
    public class ClassService : IClassService
    {
        private readonly IGenericRepository<Class> _classRepo;
        public ClassService(IGenericRepository<Class> classRepo) => _classRepo = classRepo;

        public async Task<IEnumerable<Class>> GetAllClassesAsync() => await _classRepo.GetAllAsync();
        public async Task<bool> CreateClassAsync(Class newClass) { await _classRepo.AddAsync(newClass); return await _classRepo.SaveChangesAsync(); }
    }
}
