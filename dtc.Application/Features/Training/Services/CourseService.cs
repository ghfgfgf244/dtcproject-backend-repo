using dtc.Application.Features.Training.Interfaces;
using dtc.Application.Features.Training.DTOs;
using dtc.Application.Features.Training.Interfaces;
using dtc.Application.Features.Training.DTOs;
using dtc.Domain.Entities.Training;
using dtc.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dtc.Application.Features.Training.Services
{
    public class CourseService : ICourseService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CourseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CourseResponseDto> CreateCourseAsync(CreateCourseRequestDto request, Guid adminId)
        {
            var course = new Course(
                request.CenterId,
                request.CourseName,
                request.LicenseType,
                request.DurationInWeeks,
                request.MaxStudents,
                request.ThumbnailUrl,
                request.Description,
                request.Price,
                adminId
            );

            await _unitOfWork.Courses.AddAsync(course);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(course);
        }

        public async Task<CourseResponseDto> UpdateCourseAsync(Guid courseId, UpdateCourseRequestDto request, Guid adminId)
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(courseId);
            if (course == null)
                throw new Exception("Course not found");

            var changed = false;
            changed |= course.UpdateInfo(request.CourseName, request.Description, request.ThumbnailUrl, adminId);
            
            if (request.Price.HasValue)
                changed |= course.ChangePrice(request.Price.Value, adminId);
                
            if (request.MaxStudents.HasValue)
                changed |= course.ChangeCapacity(request.MaxStudents.Value, adminId);
                
            if (request.DurationInWeeks.HasValue)
                changed |= course.ChangeDuration(request.DurationInWeeks.Value, adminId);

            if (changed)
            {
                await _unitOfWork.Courses.UpdateAsync(course);
                await _unitOfWork.SaveChangesAsync();
            }

            return MapToDto(course);
        }

        public async Task DeactivateCourseAsync(Guid courseId, Guid adminId)
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(courseId);
            if (course == null)
                throw new Exception("Course not found");

            // Check if there are active classes for this course's terms
            var terms = await _unitOfWork.Terms.FindAsync(t => t.CourseId == courseId);
            var termIds = terms.Select(t => t.Id).ToList();
            
            var classes = await _unitOfWork.Classes.FindAsync(c => termIds.Contains(c.TermId));
            var hasActiveClasses = classes.Any(c => c.Status == dtc.Domain.Entities.ClassStatus.InProgress);

            course.Deactivate(hasActiveClasses, adminId);
            await _unitOfWork.Courses.UpdateAsync(course);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<CourseResponseDto>> GetAllCoursesAsync()
        {
            var courses = await _unitOfWork.Courses.GetAllAsync();
            return courses.Select(MapToDto);
        }

        public async Task<IEnumerable<CourseResponseDto>> GetAvailableCoursesAsync()
        {
            var courses = await _unitOfWork.Courses.FindAsync(c => c.IsActive);
            return courses.Select(MapToDto);
        }

        public async Task<CourseResponseDto> GetCourseDetailAsync(Guid courseId)
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(courseId);
            if (course == null)
                throw new Exception("Course not found");

            return MapToDto(course);
        }

        private CourseResponseDto MapToDto(Course course)
        {
            return new CourseResponseDto
            {
                Id = course.Id,
                CenterId = course.CenterId,
                CourseName = course.CourseName,
                LicenseType = course.LicenseType.ToString(),
                DurationInWeeks = course.DurationInWeeks,
                MaxStudents = course.MaxStudents,
                ThumbnailUrl = course.ThumbnailUrl,
                Description = course.Description,
                Price = course.Price,
                IsActive = course.IsActive,
                CreatedAt = course.CreatedAt
            };
        }
    }
}
