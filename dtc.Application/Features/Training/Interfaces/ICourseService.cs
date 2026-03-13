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
        Task DeactivateCourseAsync(Guid courseId, Guid adminId);
        
        Task<IEnumerable<CourseResponseDto>> GetAllCoursesAsync();
        Task<IEnumerable<CourseResponseDto>> GetAvailableCoursesAsync();
        Task<CourseResponseDto> GetCourseDetailAsync(Guid courseId);
    }
}
