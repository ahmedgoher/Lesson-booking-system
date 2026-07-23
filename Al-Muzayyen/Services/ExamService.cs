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

        public async Task UpdateExamAsync(Exam exam)
        {
            _examRepository.Update(exam);
            await _examRepository.SaveAsync();
        }

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