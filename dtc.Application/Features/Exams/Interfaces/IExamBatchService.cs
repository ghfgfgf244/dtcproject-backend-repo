using dtc.Application.Features.Exams.Interfaces;
using dtc.Application.Features.Exams.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace dtc.Application.Features.Exams.Interfaces
{
    public interface IExamBatchService
    {
        Task<ExamBatchResponseDto> CreateExamBatchAsync(CreateExamBatchRequestDto request, Guid adminId);
        Task<ExamBatchResponseDto> UpdateExamBatchAsync(Guid id, UpdateExamBatchRequestDto request, Guid adminId);
        Task<bool> DeleteExamBatchAsync(Guid id, Guid adminId);
        Task<ExamBatchResponseDto> GetExamBatchDetailAsync(Guid id);
        Task<IEnumerable<ExamBatchResponseDto>> GetAllExamBatchesAsync();
        Task<bool> UpdateExamBatchStatusAsync(Guid id, UpdateExamBatchStatusRequestDto request, Guid adminId);
    }
}
