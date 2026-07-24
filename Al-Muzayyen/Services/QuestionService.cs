using Al_Muzayyen.Models;
using Al_Muzayyen.Repositories;
using Al_Muzayyen.Viewmodel;

namespace Al_Muzayyen.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly IQuestionRepository _questionRepository;

        public QuestionService(IQuestionRepository questionRepository)
        {
            _questionRepository = questionRepository;
        }
        public async Task<IEnumerable<QuestionListVM>> GetQuestionsByExamIdAsync(int examId)
        {
            return await _questionRepository.GetQuestionsByExamIdAsync(examId);
        }
        private async Task UpdateExamStatusAsync(int examId)
        {
            var exam = await _questionRepository.GetExamByIdAsync(examId);

            if (exam == null)
                return;

            var totalQuestionsMarks =
                await _questionRepository.GetExamMarksSumAsync(examId);

            exam.IsActive = totalQuestionsMarks == exam.TotalMarks;

            await _questionRepository.UpdateExamAsync(exam);
        }

        public async Task UpdateQuestionAsync(QuestionViewModel model)
        {
            var question = await _questionRepository.GetByIdAsync(model.Id);

            if (question == null)
                throw new Exception("السؤال غير موجود");

            var exam = await _questionRepository.GetExamByIdAsync(question.ExamId);

            if (exam == null)
                throw new Exception("الامتحان غير موجود");

            // مجموع الدرجات بعد التعديل
            var currentMarks = await _questionRepository.GetExamMarksSumAsync(question.ExamId);

            currentMarks = currentMarks - question.Mark + model.Mark;

            if (currentMarks > exam.TotalMarks)
            {
                throw new Exception(
                    $"لا يمكن تعديل السؤال، لأن مجموع الدرجات سيصبح {currentMarks} من {exam.TotalMarks}");
            }

            question.QuestionText = model.QuestionText;
            question.Mark = model.Mark;

            if (Enum.TryParse<QuestionType>(model.Type, out var parsedType))
            {
                question.Type = parsedType;
            }

            if (!string.IsNullOrEmpty(model.ImageUrl))
            {
                question.ImageUrl = model.ImageUrl;
            }

            if (question.Type == QuestionType.MCQ)
            {
                var options = new[]
                {
            new { Text = model.OptionA ?? "", IsCorrect = model.CorrectAnswer == "A" },
            new { Text = model.OptionB ?? "", IsCorrect = model.CorrectAnswer == "B" },
            new { Text = model.OptionC ?? "", IsCorrect = model.CorrectAnswer == "C" },
            new { Text = model.OptionD ?? "", IsCorrect = model.CorrectAnswer == "D" }
        };

                for (int i = 0; i < options.Length; i++)
                {
                    if (i < question.Options.Count)
                    {
                        question.Options[i].OptionText = options[i].Text;
                        question.Options[i].IsCorrect = options[i].IsCorrect;
                    }
                    else
                    {
                        question.Options.Add(new QuestionOption
                        {
                            OptionText = options[i].Text,
                            IsCorrect = options[i].IsCorrect
                        });
                    }
                }
            }
            else
            {
                bool isTrueCorrect = model.CorrectAnswer == "True";

                if (question.Options.Count >= 2)
                {
                    question.Options[0].OptionText = "صح";
                    question.Options[0].IsCorrect = isTrueCorrect;

                    question.Options[1].OptionText = "خطأ";
                    question.Options[1].IsCorrect = !isTrueCorrect;
                }
                else
                {
                    question.Options.Clear();

                    question.Options.Add(new QuestionOption
                    {
                        OptionText = "صح",
                        IsCorrect = isTrueCorrect
                    });

                    question.Options.Add(new QuestionOption
                    {
                        OptionText = "خطأ",
                        IsCorrect = !isTrueCorrect
                    });
                }
            }

            await _questionRepository.UpdateAsync(question);

            await UpdateExamStatusAsync(question.ExamId);
        }
        public async Task DeleteQuestionAsync(int id)
        {
            var examId = await _questionRepository.DeleteQuestionAsync(id);

            if (examId.HasValue)
            {
                await UpdateExamStatusAsync(examId.Value);
            }
        }
        public async Task AddQuestionAsync(QuestionViewModel model)
        {
            var exam = await _questionRepository.GetExamByIdAsync(model.ExamId);

            if (exam == null)
                throw new Exception("الامتحان غير موجود");

            var currentMarks = await _questionRepository.GetExamMarksSumAsync(model.ExamId);

            if (currentMarks + model.Mark > exam.TotalMarks)
            {
                throw new Exception(
                    $"لا يمكن إضافة السؤال، لأن مجموع الدرجات سيصبح {currentMarks + model.Mark} من {exam.TotalMarks}");
            }

            var question = new Question
            {
                QuestionText = model.QuestionText,
                ImageUrl = model.ImageUrl,
                Mark = model.Mark,
                ExamId = model.ExamId,
                Type = model.Type == "TrueFalse"
                    ? QuestionType.TrueFalse
                    : QuestionType.MCQ
            };

            if (question.Type == QuestionType.MCQ)
            {
                question.Options.Add(new QuestionOption
                {
                    OptionText = model.OptionA,
                    IsCorrect = model.CorrectAnswer == "A"
                });

                question.Options.Add(new QuestionOption
                {
                    OptionText = model.OptionB,
                    IsCorrect = model.CorrectAnswer == "B"
                });

                question.Options.Add(new QuestionOption
                {
                    OptionText = model.OptionC,
                    IsCorrect = model.CorrectAnswer == "C"
                });

                question.Options.Add(new QuestionOption
                {
                    OptionText = model.OptionD,
                    IsCorrect = model.CorrectAnswer == "D"
                });
            }
            else
            {
                bool isTrue = model.CorrectAnswer.Equals("true", StringComparison.OrdinalIgnoreCase);

                question.Options.Add(new QuestionOption
                {
                    OptionText = "صح",
                    IsCorrect = isTrue
                });

                question.Options.Add(new QuestionOption
                {
                    OptionText = "خطأ",
                    IsCorrect = !isTrue
                });
            }

            await _questionRepository.AddAsync(question);
            await _questionRepository.SaveAsync();

            await UpdateExamStatusAsync(model.ExamId);
        }
    }
}