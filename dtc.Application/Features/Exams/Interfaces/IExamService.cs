using dtc.Application.Features.Exams.Interfaces;
using dtc.Application.Features.Exams.DTOs;
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
    }
}
