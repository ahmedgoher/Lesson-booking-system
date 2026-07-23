using Al_Muzayyen.Models;
using Al_Muzayyen.Viewmodel;
using Microsoft.EntityFrameworkCore;

namespace Al_Muzayyen.Repositories
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly AppDbContext _context;

        public QuestionRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task UpdateAsync(Question question)
        {
            _context.Questions.Update(question);
            await _context.SaveChangesAsync();
        }
        public async Task<Question?> GetByIdAsync(int id)
        {
            return await _context.Questions
                                 .Include(q => q.Options) // 👈 ضروري لكي لا يمسح الاختيارات أو يحدث أخطاء
                                 .FirstOrDefaultAsync(q => q.Id == id);
        }

        public async Task<IEnumerable<QuestionListVM>> GetQuestionsByExamIdAsync(int examId)
        {
            return await _context.Questions
                .Include(q => q.Options)
                .Where(q => q.ExamId == examId)
                .AsNoTracking()
                .Select(q => new QuestionListVM
                {
                    Id = q.Id,
                    QuestionText = q.QuestionText,
                    ImageUrl = q.ImageUrl,
                    Mark = q.Mark,
                    Type = q.Type,
                    Options = q.Options.ToList()
                })
                .ToListAsync();
        }
        public async Task AddAsync(Question question)
        {
            await _context.Questions.AddAsync(question);
        }
        public async Task DeleteQuestionAsync(int id)
        {
            // 1. جلب السؤال مع كافة الخيارات التابعة له
            var question = await _context.Questions
                                         .Include(q => q.Options)
                                         .FirstOrDefaultAsync(q => q.Id == id);

            if (question != null)
            {
                // 2. مسح الخيارات أولاً من قاعدة البيانات
                if (question.Options != null && question.Options.Any())
                {
                    _context.QuestionOptions.RemoveRange(question.Options);
                }

                // 3. مسح السؤال نفسه
                _context.Questions.Remove(question);

                // 4. حفظ التغييرات
                await _context.SaveChangesAsync();
            }
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}