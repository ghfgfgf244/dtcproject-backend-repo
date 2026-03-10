using dtc.Application.DTOs.Exams;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace dtc.Application.Interfaces.Exams
{
    public interface IQuestionService
    {
        Task<QuestionResponseDto> CreateQuestionAsync(CreateQuestionRequestDto request);
        Task<QuestionResponseDto> UpdateQuestionAsync(int id, UpdateQuestionRequestDto request);
        Task<bool> DeleteQuestionAsync(int id);
        Task<QuestionResponseDto> GetQuestionDetailAsync(int id);
        Task<IEnumerable<QuestionResponseDto>> GetAllQuestionsAsync();
    }
}
