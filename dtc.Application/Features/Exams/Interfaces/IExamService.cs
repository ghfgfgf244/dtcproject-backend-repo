using dtc.Application.Features.Exams.Interfaces;
using dtc.Application.Features.Exams.DTOs;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace dtc.Application.Features.Exams.Interfaces
{
    public interface IExamService
    {
        Task<ExamResponseDto> CreateExamAsync(CreateExamRequestDto request, Guid adminId);
        Task<ExamResponseDto> UpdateExamAsync(Guid id, UpdateExamRequestDto request, Guid adminId);
        Task<bool> DeleteExamAsync(Guid id, Guid adminId);
        Task<ExamResponseDto> GetExamDetailAsync(Guid id);
        Task<IEnumerable<ExamResponseDto>> GetAllExamsAsync();
        Task<IEnumerable<ExamResponseDto>> GetExamsByCourseAsync(Guid courseId);
        Task<IEnumerable<ExamResponseDto>> GetExamsByBatchAsync(Guid batchId);
        Task<object> GetExamResultsAsync(Guid examId);
        Task<bool> UpdateExamResultAsync(Guid resultId, UpdateExamResultRequestDto request, Guid adminId);
        Task<bool> EnterBulkExamResultsAsync(BulkExamResultRequestDto request, Guid adminId);
        Task<object> GetMyExamResultsAsync(Guid studentId);
        Task<IEnumerable<ExamResponseDto>> GetMyExamsAsync(Guid studentId);
        Task<ExamScoreboardResponseDto> GetExamScoreboardAsync(ExamScoreboardQueryDto query);
        Task<ExamScoreboardItemDto> UpsertStudentExamScoresAsync(UpsertStudentExamScoresRequestDto request, Guid adminId);
        Task<ExamScoreImportResponseDto> ImportExamScoresAsync(ExamScoreImportRequestDto request, IFormFile file, Guid adminId);
        Task<byte[]> GenerateScoreImportTemplateAsync(Guid courseId, Guid termId, Guid examBatchId);
    }
}
