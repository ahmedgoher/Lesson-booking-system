using Al_Muzayyen.Models;
using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;

namespace Al_Muzayyen.Repositories
{
    public class StudentRepository:IStudentRepository
    {
        private readonly AppDbContext _context;
        public StudentRepository(AppDbContext context)
        {
            _context = context;

        }
        public async Task<Student?> GetByUserIdAsync(string userId)
        {
            if (int.TryParse(userId, out int studentId))
            {
                return await _context.Students
                    .FirstOrDefaultAsync(x => x.Id == studentId);
            }

            return null;
        }
    }
}
