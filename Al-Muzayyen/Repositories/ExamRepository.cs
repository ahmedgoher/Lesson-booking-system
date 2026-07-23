using Al_Muzayyen.Models;
using Al_Muzayyen.Viewmodel;
using Microsoft.EntityFrameworkCore;

namespace Al_Muzayyen.Repositories
{
    public class ExamRepository : IExamRepository
    {
        private readonly AppDbContext _context;

        public ExamRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Exam>> GetAllWithDetailsAsync()
        {
            return await _context.Exams
                .Include(e => e.Class)
                .Include(e => e.Questions)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Exam?> GetByIdAsync(int id)
        {
            return await _context.Exams
                .Include(e => e.Class)
                .Include(e => e.Questions)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task AddAsync(Exam exam)
        {
            await _context.Exams.AddAsync(exam);
        }

        public void Update(Exam exam)
        {
            _context.Exams.Update(exam);
        }

        public void Delete(Exam exam)
        {
            _context.Exams.Remove(exam);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<ExamViewModel>> GetAllExamsViewModelsAsync()
        {
            return await _context.Exams
                .AsNoTracking() // مسحنا الـ Include لأن Select بتقوم بالواجب وزيادة
                .Select(e => new ExamViewModel
                {
                    Id = e.Id,
                    Title = e.Title,
                    Description = e.Description ?? "", // قراءة الوصف الحقيقي
                    GradeId = e.ClassId,
                    GradeName = e.Class != null ? e.Class.Name : "",
                    Date = e.CreatedAt,
                    Duration = e.DurationMinutes,
                    OpenDate = e.StartExamTime, // تأكد من مطابقة اسم الحرف (كبير S)
                    CloseDate = e.EndExamTime,   // تأكد من مطابقة اسم الحرف (كبير E)
                    Status = e.IsActive ? "Active" : "Closed",
                    QuestionsCount = e.Questions.Count,

                    // 🟢 قراءة البيانات الحقيقية المخزنة في قاعدة البيانات
                    TotalMarks = e.TotalMarks,
                    PassingMarks = e.PassingMarks,
                    MaxAttempts = e.MaxAttempts,
                    RandomQuestions = e.RandomQuestions,
                    ShuffleAnswers = e.ShuffleAnswers,
                    AllowReview = e.AllowReview,
                    ShowResult = e.ShowResult,
                    
                })
                .ToListAsync();
        }
    }
}