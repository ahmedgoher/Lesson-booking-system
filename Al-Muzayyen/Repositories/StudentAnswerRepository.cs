using Al_Muzayyen.Models;
using Microsoft.EntityFrameworkCore;

namespace Al_Muzayyen.Repositories
{
    public class StudentAnswerRepository : IStudentAnswerRepository
    {
        private readonly AppDbContext _context;

        public StudentAnswerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<StudentAnswer?> GetAnswerAsync(int studentExamId, int questionId)
        {
            return await _context.StudentAnswers
                .FirstOrDefaultAsync(x =>
                    x.StudentExamId == studentExamId &&
                    x.QuestionId == questionId);
        }
        public async Task<QuestionOption?> GetOptionAsync(int optionId)
        {
            return await _context.QuestionOptions
                .FirstOrDefaultAsync(x => x.Id == optionId);
        }

        public async Task<Question?> GetQuestionAsync(int questionId)
        {
            return await _context.Questions
                .FirstOrDefaultAsync(x => x.Id == questionId);
        }
        public async Task AddAsync(StudentAnswer answer)
        {
            await _context.StudentAnswers.AddAsync(answer);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}