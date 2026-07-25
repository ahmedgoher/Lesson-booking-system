using Al_Muzayyen.Models;
using Al_Muzayyen.Viewmodel;

namespace Al_Muzayyen.Services
{
    public interface IExamService
    {
        Task<IEnumerable<Exam>> GetAllExamsAsync();
        Task<Exam?> GetExamByIdAsync(int id);
        Task CreateExamAsync(Exam exam);
        //Task UpdateExamAsync(Exam exam);
        Task<bool> UpdateExamAsync(ExamViewModel model);
        Task<StudentExam> StartExamAsync(int examId, int studentId);
        Task UpdateExam(Exam exam);

        Task DeleteExamAsync(int id);
        Task<IEnumerable<ExamViewModel>> GetAllExamsViewModelsAsync();
    }
}