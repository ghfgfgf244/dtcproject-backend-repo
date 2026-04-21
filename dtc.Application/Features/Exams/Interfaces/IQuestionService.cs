using dtc.Application.Features.Exams.DTOs;
using dtc.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace dtc.Application.Features.Exams.Interfaces
{
    public interface IQuestionService
    {
        Task<QuestionResponseDto> CreateQuestionAsync(CreateQuestionRequestDto request);
        Task<QuestionResponseDto> UpdateQuestionAsync(int id, UpdateQuestionRequestDto request);
        Task<bool> DeleteQuestionAsync(int id);
        Task<QuestionResponseDto> GetQuestionDetailAsync(int id);
        Task<IEnumerable<QuestionResponseDto>> GetAllQuestionsAsync(string? category = null);
        Task<IEnumerable<CommonMistakeQuestionDto>> GetCommonMistakesAsync(string? category = null, ExamLevel? level = null, int limit = 10);
        Task<QuestionImportResponseDto> ImportQuestionsAsync(IFormFile file);
        Task<byte[]> GenerateImportTemplateAsync();
    }
}
