using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using dtc.Application.Features.Training.Interfaces;
using dtc.Application.Features.Training.DTOs;

namespace dtc.Application.Features.Training.Interfaces
{
    public interface IClassService
    {
        Task<ClassResponseDto> CreateClassAsync(CreateClassRequestDto request, Guid adminId);
        Task<ClassResponseDto> UpdateClassAsync(Guid classId, UpdateClassRequestDto request, Guid adminId);
        
        Task<IEnumerable<ClassResponseDto>> GetAllClassesAsync();
        Task<IEnumerable<ClassResponseDto>> GetClassesByTermAsync(Guid termId);
        Task<ClassResponseDto> GetClassDetailAsync(Guid classId);
        Task<bool> DeleteClassAsync(Guid classId, Guid adminId);
        Task<bool> AssignTeachersToClassAsync(Guid classId, AssignTeachersRequestDto request, Guid adminId);
        Task<bool> AssignStudentsToClassAsync(Guid classId, AssignStudentsRequestDto request, Guid adminId);
        Task<bool> RemoveStudentFromClassAsync(Guid classId, Guid studentId, Guid adminId);
        Task<bool> TransferStudentAsync(Guid classId, Guid studentId, TransferStudentRequestDto request, Guid adminId);
        Task<IEnumerable<ClassStudentResponseDto>> GetAvailableStudentsAsync(Guid classId);
        Task<AutoAssignClassesResponseDto> AutoAssignClassesAsync(AutoAssignClassesRequestDto request, Guid adminId);
        Task<AutoAssignClassesExplainResponseDto> PreviewAutoAssignClassesAsync(AutoAssignClassesRequestDto request);
        
        Task<IEnumerable<ClassResponseDto>> GetClassesByInstructorAsync(Guid instructorId);
        Task<IEnumerable<ClassStudentResponseDto>> GetClassStudentsAsync(Guid classId);
    }
}
