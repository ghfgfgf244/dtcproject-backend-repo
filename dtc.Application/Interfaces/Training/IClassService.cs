using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using dtc.Application.DTOs.Training.Classes;

namespace dtc.Application.Interfaces.Training
{
    public interface IClassService
    {
        Task<ClassResponseDto> CreateClassAsync(CreateClassRequestDto request, Guid adminId);
        Task<ClassResponseDto> UpdateClassAsync(Guid classId, UpdateClassRequestDto request, Guid adminId);
        
        Task<IEnumerable<ClassResponseDto>> GetAllClassesAsync();
        Task<ClassResponseDto> GetClassDetailAsync(Guid classId);
        Task<bool> DeleteClassAsync(Guid classId, Guid adminId);
        Task<bool> AssignTeachersToClassAsync(Guid classId, AssignTeachersRequestDto request, Guid adminId);
        Task<bool> AssignStudentsToClassAsync(Guid classId, AssignStudentsRequestDto request, Guid adminId);
    }
}
