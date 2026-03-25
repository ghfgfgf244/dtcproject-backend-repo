using dtc.Application.Features.Training.Interfaces;
using dtc.Application.Features.Training.DTOs;
using dtc.Application.Features.Training.Interfaces;
using dtc.Application.Features.Training.DTOs;
using dtc.Application.Features.Email.Interfaces;
using dtc.Application.Features.Notifications.Interfaces;
using dtc.Domain.Entities;
using dtc.Domain.Entities.Terms;
using dtc.Domain.Entities.Collaborators;
using dtc.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dtc.Application.Features.Training.Services
{
    public class CourseRegistrationService : ICourseRegistrationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;
        private readonly IEmailService _emailService;

        public CourseRegistrationService(
            IUnitOfWork unitOfWork,
            INotificationService notificationService,
            IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            _emailService = emailService;
        }

        public async Task<CourseRegistrationResponseDto> RegisterCourseAsync(RegisterCourseRequestDto request, Guid studentId)
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId);
            if (course == null || !course.IsActive)
                throw new Exception("Course not found or inactive");

            var registration = new CourseRegistration(request.CourseId, studentId, request.TotalFee, request.Notes, studentId);
            
            await _unitOfWork.CourseRegistrations.AddAsync(registration);
            await _unitOfWork.SaveChangesAsync();

            // Handle Referral Code if provided
            if (!string.IsNullOrWhiteSpace(request.ReferralCode))
            {
                try
                {
                    var referralQuery = await _unitOfWork.ReferralCodes.FindAsync(c => c.Code == request.ReferralCode.Trim().ToUpper() && c.IsActive);
                    var referralCode = referralQuery.FirstOrDefault();
                    
                    if (referralCode != null)
                    {
                        // 1. Create registration link
                        var referralReg = new ReferralRegistration(referralCode.Id, studentId);
                        await _unitOfWork.ReferralRegistrations.AddAsync(referralReg);
                        
                        // 2. Increase usage count
                        referralCode.IncreaseUsage(studentId);
                        await _unitOfWork.ReferralCodes.UpdateAsync(referralCode);
                        
                        await _unitOfWork.SaveChangesAsync();

                        // 3. Notify Collaborator
                        var studentQuery = await _unitOfWork.Users.GetByIdAsync(studentId);
                        var studentName = studentQuery?.FullName ?? "Học viên mới";
                        
                        await _notificationService.CreateForUserAsync(
                            referralCode.CollaboratorId,
                            "Mã giới thiệu được sử dụng",
                            $"{studentName} đã sử dụng mã '{referralCode.Code}' của bạn để đăng ký '{course.CourseName}'.",
                            NotificationType.Referral);
                    }
                }
                catch { /* Referral logic side-effect should not fail registration */ }
            }

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

            // Side-effect: notify student when approved
            if (request.Status == CourseRegistrationStatus.Approved)
            {
                try
                {
                    var student = await _unitOfWork.Users.GetByIdAsync(registration.UserId);
                    var course = await _unitOfWork.Courses.GetByIdAsync(registration.CourseId);
                    if (student != null && course != null)
                    {
                        await _notificationService.CreateForUserAsync(
                            student.Id,
                            "Đăng ký khóa học thành công",
                            $"Bạn đã được chấp nhận vào khóa học '{course.CourseName}'.",
                            NotificationType.Registration);

                        var center = await _unitOfWork.Centers.GetByIdAsync(course.CenterId);
                        await _emailService.SendCourseRegistrationConfirmationAsync(
                            student.Email.Value,
                            student.FullName,
                            course.CourseName,
                            center?.CenterName ?? "DTC");
                    }
                }
                catch { /* không làm hỏng luồng chính */ }
            }
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
