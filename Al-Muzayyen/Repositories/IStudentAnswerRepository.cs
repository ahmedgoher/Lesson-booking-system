using Al_Muzayyen.Models;

namespace Al_Muzayyen.Repositories
{
    public interface IStudentAnswerRepository
    {
        Task<StudentAnswer?> GetAnswerAsync(int studentExamId, int questionId);

        Task AddAsync(StudentAnswer answer);

        Task SaveAsync();
        Task<QuestionOption?> GetOptionAsync(int optionId);

        Task<Question?> GetQuestionAsync(int questionId);
    }
}