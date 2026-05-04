using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using dtc.Application.Features.Training.Interfaces;
using dtc.Application.Features.Training.DTOs;

namespace dtc.Application.Features.Training.Interfaces
{
    public interface ICourseService
    {
        Task<CourseResponseDto> CreateCourseAsync(CreateCourseRequestDto request, Guid adminId);
        Task<CourseResponseDto> UpdateCourseAsync(Guid courseId, UpdateCourseRequestDto request, Guid adminId);
        Task ActivateCourseAsync(Guid courseId, Guid adminId);
        Task DeactivateCourseAsync(Guid courseId, Guid adminId);
        Task DeleteCourseAsync(Guid courseId, Guid adminId);
        
        Task<IEnumerable<CourseResponseDto>> GetAllCoursesAsync();
        Task<IEnumerable<CourseResponseDto>> GetAvailableCoursesAsync();
        Task<IEnumerable<CourseResponseDto>> GetCoursesByCenterAsync(Guid centerId);
        Task<CourseResponseDto> GetCourseDetailAsync(Guid courseId);
    }
}
