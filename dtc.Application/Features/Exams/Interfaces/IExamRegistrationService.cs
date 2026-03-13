using dtc.Application.Features.Exams.Interfaces;
using dtc.Application.Features.Exams.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace dtc.Application.Features.Exams.Interfaces
{
    public interface IExamRegistrationService
    {
        Task<ExamRegistrationResponseDto> RegisterAsync(CreateExamRegistrationRequestDto request);
        Task<bool> UpdateStatusAsync(Guid id, UpdateExamRegistrationStatusRequestDto request, Guid adminId);
        Task<bool> MarkAsPaidAsync(Guid id, Guid adminId);
        Task<IEnumerable<ExamRegistrationResponseDto>> GetByExamBatchAsync(Guid examBatchId);
        Task<IEnumerable<ExamRegistrationResponseDto>> GetByStudentAsync(Guid studentId);
    }
}
