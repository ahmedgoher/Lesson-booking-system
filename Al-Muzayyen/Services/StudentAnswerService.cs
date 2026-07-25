using Al_Muzayyen.Models;
using Al_Muzayyen.Repositories;

namespace Al_Muzayyen.Services
{
    public class StudentAnswerService : IStudentAnswerService
    {
        private readonly IStudentAnswerRepository _repository;

        public StudentAnswerService(IStudentAnswerRepository repository)
        {
            _repository = repository;
        }

        public async Task SaveAnswerAsync(
            int studentExamId,
            int questionId,
            int optionId)
        {
            var answer =
                await _repository.GetAnswerAsync(studentExamId, questionId);

            var option =
                await _repository.GetOptionAsync(optionId);

            var question =
                await _repository.GetQuestionAsync(questionId);

            if (option == null || question == null)
                throw new Exception("بيانات السؤال غير موجودة");

            if (answer == null)
            {
                answer = new StudentAnswer
                {
                    StudentExamId = studentExamId,
                    QuestionId = questionId,
                    QuestionOptionId = optionId,
                    IsCorrect = option.IsCorrect,
                    EarnedMarks = option.IsCorrect ? question.Mark : 0
                };

                await _repository.AddAsync(answer);
            }
            else
            {
                answer.QuestionOptionId = optionId;
                answer.IsCorrect = option.IsCorrect;
                answer.EarnedMarks = option.IsCorrect
                    ? question.Mark
                    : 0;
            }

            await _repository.SaveAsync();
        }
    }
}