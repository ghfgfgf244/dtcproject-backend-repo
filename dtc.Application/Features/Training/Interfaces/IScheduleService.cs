using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using dtc.Application.Features.Training.Interfaces;
using dtc.Application.Features.Training.DTOs;

namespace dtc.Application.Features.Training.Interfaces
{
    public interface IScheduleService
    {
        Task<ClassScheduleResponseDto> CreateScheduleAsync(CreateClassScheduleRequestDto request, Guid adminId);
        Task<ClassScheduleResponseDto> UpdateScheduleAsync(Guid id, UpdateClassScheduleRequestDto request, Guid adminId);
        Task<bool> DeleteScheduleAsync(Guid id, Guid adminId);
        Task<ClassScheduleResponseDto> GetScheduleDetailAsync(Guid id);
        Task<IEnumerable<ClassScheduleResponseDto>> GetSchedulesByClassAsync(Guid classId);
        Task<bool> AssignLocationAsync(Guid id, AssignLocationRequestDto request, Guid adminId);
    }
}
