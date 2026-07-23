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

        public async Task UpdateQuestionAsync(QuestionViewModel model)
        {
            var question = await _questionRepository.GetByIdAsync(model.Id);

            if (question == null)
            {
                throw new Exception("السؤال غير موجود");
            }

            question.QuestionText = model.QuestionText;
            question.Mark = (int)model.Mark;

            if (Enum.TryParse<QuestionType>(model.Type, out var parsedType))
            {
                question.Type = parsedType;
            }

            if (!string.IsNullOrEmpty(model.ImageUrl))
            {
                question.ImageUrl = model.ImageUrl;
            }

            // ----------------------------------------------------
            // تحديث الخيارات (Options) بدون استخدام Clear()
            // ----------------------------------------------------
            if (question.Type == QuestionType.MCQ)
            {
                var optionsList = new[]
                {
            new { Text = model.OptionA ?? "", IsCorrect = model.CorrectAnswer == "A" },
            new { Text = model.OptionB ?? "", IsCorrect = model.CorrectAnswer == "B" },
            new { Text = model.OptionC ?? "", IsCorrect = model.CorrectAnswer == "C" },
            new { Text = model.OptionD ?? "", IsCorrect = model.CorrectAnswer == "D" }
        };

                // تحديث الخيارات الموجودة في الداتا بيز
                for (int i = 0; i < optionsList.Length; i++)
                {
                    if (i < question.Options.Count)
                    {
                        // تحديث الخيار الحالي
                        question.Options[i].OptionText = optionsList[i].Text;
                        question.Options[i].IsCorrect = optionsList[i].IsCorrect;
                    }
                    else
                    {
                        // إذا كانت الخيارات القديمة أقل من 4، أضف الخيار الجديد
                        question.Options.Add(new QuestionOption
                        {
                            OptionText = optionsList[i].Text,
                            IsCorrect = optionsList[i].IsCorrect
                        });
                    }
                }
            }
            else if (question.Type == QuestionType.TrueFalse)
            {
                bool isTrueCorrect = model.CorrectAnswer?.ToLower() == "true";

                if (question.Options.Count >= 2)
                {
                    question.Options[0].OptionText = "True";
                    question.Options[0].IsCorrect = isTrueCorrect;

                    question.Options[1].OptionText = "False";
                    question.Options[1].IsCorrect = !isTrueCorrect;
                }
                else
                {
                    question.Options.Clear(); // إذا كانت أقل من 2 لأي سبب
                    question.Options.Add(new QuestionOption { OptionText = "True", IsCorrect = isTrueCorrect });
                    question.Options.Add(new QuestionOption { OptionText = "False", IsCorrect = !isTrueCorrect });
                }
            }

            await _questionRepository.UpdateAsync(question);
        }
        public async Task DeleteQuestionAsync(int id)
        {
            await _questionRepository.DeleteQuestionAsync(id);
        }
        public async Task AddQuestionAsync(QuestionViewModel model)
        {
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
                question.Options.Add(new QuestionOption
                {
                    OptionText = "صح",
                    IsCorrect = model.CorrectAnswer == "True"
                });

                question.Options.Add(new QuestionOption
                {
                    OptionText = "خطأ",
                    IsCorrect = model.CorrectAnswer == "False"
                });
            }

            await _questionRepository.AddAsync(question);
            await _questionRepository.SaveAsync();
        }
    }
}