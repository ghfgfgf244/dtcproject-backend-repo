using dtc.Application.Features.Training.Interfaces;
using dtc.Application.Features.Training.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace dtc.Application.Features.Training.Interfaces
{
    public interface IStudentEvaluationService
    {
        Task<StudentEvaluationResponseDto> CreateEvaluationAsync(Guid instructorId, CreateStudentEvaluationRequestDto request);
        Task<StudentEvaluationResponseDto> UpdateEvaluationAsync(Guid id, Guid instructorId, UpdateStudentEvaluationRequestDto request);
        Task<IEnumerable<StudentEvaluationResponseDto>> GetEvaluationsForStudentAsync(Guid studentId);
        Task<IEnumerable<StudentEvaluationResponseDto>> GetEvaluationsByClassAsync(Guid classId);
        Task<StudentEvaluationResponseDto> GetEvaluationByIdAsync(Guid id);
        Task<bool> DeleteEvaluationAsync(Guid id);
    }
}
