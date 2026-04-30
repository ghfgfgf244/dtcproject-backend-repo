using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using dtc.Application.Features.Training.Interfaces;
using dtc.Application.Features.Training.DTOs;

namespace dtc.Application.Features.Training.Interfaces
{
    public interface ICourseRegistrationService
    {
        Task<CourseRegistrationResponseDto> RegisterCourseAsync(RegisterCourseRequestDto request, Guid? studentId);
        Task CancelRegistrationAsync(Guid registrationId, string reason, Guid studentId);
        Task UpdateRegistrationStatusAsync(Guid registrationId, UpdateRegistrationStatusDto request, Guid adminId);
        
        Task<IEnumerable<CourseRegistrationResponseDto>> GetMyRegistrationsAsync(Guid studentId);
        Task<IEnumerable<CourseRegistrationResponseDto>> GetAllRegistrationsAsync();
        Task<CourseRegistrationPagedResponseDto> GetRegistrationsPagedAsync(CourseRegistrationPagedQueryDto query, Guid? managedCenterId = null);
        Task<CourseRegistrationResponseDto> GetRegistrationDetailAsync(Guid registrationId);
        Task<IReadOnlyCollection<CourseRegistrationTermOptionDto>> GetRegistrationTermOptionsAsync(Guid registrationId);
        Task<CourseRegistrationResponseDto> ReassignRegistrationTermAsync(Guid registrationId, Guid termId, Guid adminId);
        Task<object> GetRegistrationStatsAsync();
    }
}
