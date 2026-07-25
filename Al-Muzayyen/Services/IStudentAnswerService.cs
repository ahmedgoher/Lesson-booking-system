using Al_Muzayyen.Models;

namespace Al_Muzayyen.Services
{
    public interface IStudentAnswerService
    {
        Task SaveAnswerAsync(
            int studentExamId,
            int questionId,
            int optionId);
    }
}