using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using dtc.Application.Features.Training.Interfaces;
using dtc.Application.Features.Training.DTOs;
using Microsoft.AspNetCore.Http;

namespace dtc.Application.Features.Training.Interfaces
{
    public interface IScheduleService
    {
        Task<ClassScheduleResponseDto> CreateScheduleAsync(CreateClassScheduleRequestDto request, Guid adminId);
        Task<ClassScheduleResponseDto> UpdateScheduleAsync(Guid id, UpdateClassScheduleRequestDto request, Guid adminId);
        Task<bool> DeleteScheduleAsync(Guid id, Guid adminId);
        Task<ClassScheduleResponseDto> GetScheduleDetailAsync(Guid id);
        Task<IEnumerable<ClassScheduleResponseDto>> GetAllSchedulesAsync();
        Task<IEnumerable<ClassScheduleResponseDto>> GetSchedulesByTermAsync(Guid termId);
        Task<IEnumerable<ClassScheduleResponseDto>> GetSchedulesByClassAsync(Guid classId);
        Task<IEnumerable<ClassScheduleResponseDto>> CreateBulkSchedulesAsync(BulkCreateClassScheduleRequestDto request, Guid adminId);
        Task<ScheduleImportPreviewDto> ImportSchedulePreviewAsync(IFormFile file, Guid? defaultInstructorId = null);
        Task<bool> AssignLocationAsync(Guid id, AssignLocationRequestDto request, Guid adminId);
        Task<ScheduleConflictExplainResponseDto> ExplainConflictAsync(ScheduleConflictExplainRequestDto request);
        Task<IEnumerable<ClassScheduleResponseDto>> GetMySchedulesAsync(Guid studentId);
        Task<IEnumerable<ClassScheduleResponseDto>> GetTeachingScheduleAsync(Guid instructorId);
    }
}
