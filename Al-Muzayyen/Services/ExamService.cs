using Al_Muzayyen.Models;
using Al_Muzayyen.Repositories;
using Al_Muzayyen.Viewmodel;

namespace Al_Muzayyen.Services
{
    public class ExamService : IExamService
    {
        private readonly IExamRepository _examRepository;

        public ExamService(IExamRepository examRepository)
        {
            _examRepository = examRepository;
        }

        public async Task<IEnumerable<Exam>> GetAllExamsAsync()
        {
            return await _examRepository.GetAllWithDetailsAsync();
        }

        public async Task<Exam?> GetExamByIdAsync(int id)
        {
            return await _examRepository.GetByIdAsync(id);
        }

        public async Task CreateExamAsync(Exam exam)
        {
            await _examRepository.AddAsync(exam);
            await _examRepository.SaveAsync();
        }
        public async Task<bool> UpdateExamAsync(ExamViewModel model)
        {
            var exam = await _examRepository.GetByIdAsync(model.Id);

            if (exam == null)
                return false;

            exam.Title = model.Title;
            exam.Description = model.Description;
            exam.ClassId = model.GradeId;

            exam.DurationMinutes = model.Duration;
            exam.IsActive = model.Status == "Active";

            exam.StartExamTime = model.OpenDate ?? exam.StartExamTime;
            exam.EndExamTime = model.CloseDate ?? exam.EndExamTime;

            exam.TotalMarks = model.TotalMarks;
            exam.PassingMarks = model.PassingMarks;
            exam.MaxAttempts = model.MaxAttempts;

            exam.RandomQuestions = model.RandomQuestions;
            exam.ShuffleAnswers = model.ShuffleAnswers;
            exam.AllowReview = model.AllowReview;
            exam.ShowResult = model.ShowResult;

            // لو المستخدم غير التاريخ
            if (model.Date != default)
                exam.CreatedAt = model.Date;

            await _examRepository.SaveAsync();

            return true;
        }
        //public async Task UpdateExamAsync(Exam exam)
        //{
        //    _examRepository.Update(exam);
        //    await _examRepository.SaveAsync();
        //}

        public async Task DeleteExamAsync(int id)
        {
            var exam = await _examRepository.GetByIdAsync(id);
            if (exam != null)
            {
                _examRepository.Delete(exam);
                await _examRepository.SaveAsync();
            }
        }
        public async Task<IEnumerable<ExamViewModel>> GetAllExamsViewModelsAsync()
        {
            return await _examRepository.GetAllExamsViewModelsAsync();
        }
    }
}