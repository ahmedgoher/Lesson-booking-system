using Al_Muzayyen.Models;
using Al_Muzayyen.Viewmodel;

namespace Al_Muzayyen.Repositories
{
    public interface IExamRepository
    {
        Task<IEnumerable<Exam>> GetAllWithDetailsAsync();
        Task<Exam?> GetByIdAsync(int id);
        Task AddAsync(Exam exam);
        void Update(Exam exam);
        void Delete(Exam exam);
        Task SaveAsync();
        Task<StudentExam?> GetStudentExamAsync(int examId, int studentId);
        Task<int> GetStudentAttemptsAsync(int examId, int studentId);
        Task AddStudentExamAsync(StudentExam studentExam);
        Task<IEnumerable<ExamViewModel>> GetAllExamsViewModelsAsync(); // أضف هذا السطر
    }
}