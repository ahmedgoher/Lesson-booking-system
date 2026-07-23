using Al_Muzayyen.Models;
using Al_Muzayyen.Viewmodel;

namespace Al_Muzayyen.Repositories
{
    public interface IQuestionRepository
    {
        Task AddAsync(Question question);
        Task<IEnumerable<QuestionListVM>> GetQuestionsByExamIdAsync(int examId);
        Task SaveAsync();
        Task UpdateAsync(Question question);
        Task<Question?> GetByIdAsync(int id);
        Task DeleteQuestionAsync(int id);
    }
}