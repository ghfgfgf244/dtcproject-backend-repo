using dtc.Application.DTOs.Exams;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace dtc.Application.Interfaces.Exams
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
