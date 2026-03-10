using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using dtc.Application.DTOs.Training.Registrations;

namespace dtc.Application.Interfaces.Training
{
    public interface ICourseRegistrationService
    {
        Task<CourseRegistrationResponseDto> RegisterCourseAsync(RegisterCourseRequestDto request, Guid studentId);
        Task CancelRegistrationAsync(Guid registrationId, string reason, Guid studentId);
        Task UpdateRegistrationStatusAsync(Guid registrationId, UpdateRegistrationStatusDto request, Guid adminId);
        
        Task<IEnumerable<CourseRegistrationResponseDto>> GetMyRegistrationsAsync(Guid studentId);
        Task<IEnumerable<CourseRegistrationResponseDto>> GetAllRegistrationsAsync();
        Task<CourseRegistrationResponseDto> GetRegistrationDetailAsync(Guid registrationId);
    }
}
