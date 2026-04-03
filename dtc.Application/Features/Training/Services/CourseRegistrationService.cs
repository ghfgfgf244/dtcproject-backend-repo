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
using Microsoft.AspNetCore.Http;
using dtc.Application.Interfaces;

namespace dtc.Application.Features.Training.Services
{
    public class CourseRegistrationService : ICourseRegistrationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;
        private readonly IEmailService _emailService;
        private readonly ICloudinaryService _cloudinaryService;

        public CourseRegistrationService(
            IUnitOfWork unitOfWork,
            INotificationService notificationService,
            IEmailService emailService,
            ICloudinaryService cloudinaryService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            _emailService = emailService;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<CourseRegistrationResponseDto> RegisterCourseAsync(RegisterCourseRequestDto request, Guid studentId)
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId);
            if (course == null || !course.IsActive)
                throw new Exception("Course not found or inactive");

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // Upload documents if any
                var uploadedDocs = new List<dtc.Domain.Entities.Permissions.Document>();
                if (request.Photo != null) uploadedDocs.Add(await UploadFileAsync(request.Photo, studentId, "image"));
                if (request.IdFront != null) uploadedDocs.Add(await UploadFileAsync(request.IdFront, studentId, "image"));
                if (request.IdBack != null) uploadedDocs.Add(await UploadFileAsync(request.IdBack, studentId, "image"));

                foreach(var doc in uploadedDocs)
                {
                    await _unitOfWork.Documents.AddAsync(doc);
                }

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
                            var referralReg = new ReferralRegistration(referralCode.Id, studentId);
                            await _unitOfWork.ReferralRegistrations.AddAsync(referralReg);
                            
                            referralCode.IncreaseUsage(studentId);
                            await _unitOfWork.ReferralCodes.UpdateAsync(referralCode);
                            
                            await _unitOfWork.SaveChangesAsync();

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

                await _unitOfWork.CommitTransactionAsync();
                return MapToDto(registration);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        private async Task<dtc.Domain.Entities.Permissions.Document> UploadFileAsync(IFormFile file, Guid studentId, string resourceType)
        {
            using var stream = file.OpenReadStream();
            var (publicId, version) = await _cloudinaryService.UploadAsync(
                stream, 
                file.FileName, 
                $"user_docs/{studentId}", 
                resourceType);

            return new dtc.Domain.Entities.Permissions.Document(
                studentId,
                publicId,
                version,
                resourceType,
                file.FileName,
                System.IO.Path.GetExtension(file.FileName),
                (int)file.Length);
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

            // Side-effect: notify student when approved, rejected, or cancelled
            try
            {
                var student = await _unitOfWork.Users.GetByIdAsync(registration.UserId);
                var course = await _unitOfWork.Courses.GetByIdAsync(registration.CourseId);
                if (student != null && course != null)
                {
                    string title = "";
                    string content = "";
                    NotificationType nType = NotificationType.Registration;

                    if (request.Status == CourseRegistrationStatus.Approved)
                    {
                        title = "Đăng ký khóa học thành công";
                        content = $"Bạn đã được chấp nhận vào khóa học '{course.CourseName}'.";
                        
                        var center = await _unitOfWork.Centers.GetByIdAsync(course.CenterId);
                        await _emailService.SendCourseRegistrationConfirmationAsync(
                            student.Email.Value,
                            student.FullName,
                            course.CourseName,
                            center?.CenterName ?? "DTC");
                    }
                    else if (request.Status == CourseRegistrationStatus.Rejected)
                    {
                        title = "Từ chối hồ sơ đăng ký";
                        content = $"Hồ sơ đăng ký khóa học '{course.CourseName}' của bạn đã bị từ chối. Lý do: {request.Reason}";
                    }
                    else if (request.Status == CourseRegistrationStatus.Cancelled)
                    {
                        title = "Hủy hồ sơ đăng ký";
                        content = $"Hồ sơ đăng ký khóa học '{course.CourseName}' đã bị hủy.";
                    }

                    if (!string.IsNullOrEmpty(title))
                    {
                        await _notificationService.CreateForUserAsync(student.Id, title, content, nType);
                    }
                }
            }
            catch { /* không làm hỏng luồng chính */ }
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
