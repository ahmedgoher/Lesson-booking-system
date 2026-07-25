using Al_Muzayyen.Viewmodel;

namespace Al_Muzayyen.Services
{
    public interface IQuestionService
    {
        Task AddQuestionAsync(QuestionViewModel model);
        Task<IEnumerable<QuestionListVM>> GetQuestionsByExamIdAsync(int examId);
        Task UpdateQuestionAsync(QuestionViewModel model);
        Task DeleteQuestionAsync(int id);
        Task<List<StudentQuestionViewModel>> GetStudentQuestionsAsync(int examId);
    }
}