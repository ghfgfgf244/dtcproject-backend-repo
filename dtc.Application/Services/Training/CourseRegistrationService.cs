using dtc.Application.DTOs.Training.Registrations;
using dtc.Application.Interfaces.Training;
using dtc.Domain.Entities;
using dtc.Domain.Entities.Terms;
using dtc.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dtc.Application.Services.Training
{
    public class CourseRegistrationService : ICourseRegistrationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CourseRegistrationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CourseRegistrationResponseDto> RegisterCourseAsync(RegisterCourseRequestDto request, Guid studentId)
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId);
            if (course == null || !course.IsActive)
                throw new Exception("Course not found or inactive");

            var registration = new CourseRegistration(request.CourseId, studentId, request.TotalFee, request.Notes, studentId);
            
            await _unitOfWork.CourseRegistrations.AddAsync(registration);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(registration);
        }

        public async Task CancelRegistrationAsync(Guid registrationId, string reason, Guid studentId)
        {
            var registration = await _unitOfWork.CourseRegistrations.GetByIdAsync(registrationId);
            if (registration == null)
                throw new Exception("Registration not found");

            if (registration.UserId != studentId)
                throw new Exception("Unauthorized to cancel this registration");

            registration.Cancel(reason, studentId);
            
            await _unitOfWork.CourseRegistrations.UpdateAsync(registration);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateRegistrationStatusAsync(Guid registrationId, UpdateRegistrationStatusDto request, Guid adminId)
        {
            var registration = await _unitOfWork.CourseRegistrations.GetByIdAsync(registrationId);
            if (registration == null)
                throw new Exception("Registration not found");

            if (request.Status == CourseRegistrationStatus.Approved)
            {
                registration.Approve(adminId);
            }
            else if (request.Status == CourseRegistrationStatus.Rejected)
            {
                registration.Reject(request.Reason, adminId);
            }
            else if (request.Status == CourseRegistrationStatus.Cancelled)
            {
                registration.Cancel(request.Reason, adminId);
            }
            
            await _unitOfWork.CourseRegistrations.UpdateAsync(registration);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<CourseRegistrationResponseDto>> GetMyRegistrationsAsync(Guid studentId)
        {
            var registrations = await _unitOfWork.CourseRegistrations.FindAsync(r => r.UserId == studentId);
            return registrations.Select(MapToDto);
        }

        public async Task<IEnumerable<CourseRegistrationResponseDto>> GetAllRegistrationsAsync()
        {
            var registrations = await _unitOfWork.CourseRegistrations.GetAllAsync();
            return registrations.Select(MapToDto);
        }

        public async Task<CourseRegistrationResponseDto> GetRegistrationDetailAsync(Guid registrationId)
        {
            var registration = await _unitOfWork.CourseRegistrations.GetByIdAsync(registrationId);
            if (registration == null)
                throw new Exception("Registration not found");

            return MapToDto(registration);
        }

        private CourseRegistrationResponseDto MapToDto(CourseRegistration registration)
        {
            return new CourseRegistrationResponseDto
            {
                Id = registration.Id,
                CourseId = registration.CourseId,
                UserId = registration.UserId,
                RegistrationDate = registration.RegistrationDate,
                Status = registration.Status.ToString(),
                TotalFee = registration.TotalFee,
                Notes = registration.Notes,
                CreatedAt = registration.CreatedAt
            };
        }
    }
}
