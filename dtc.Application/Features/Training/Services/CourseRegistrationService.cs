using dtc.Application.Features.Training.DTOs;
using dtc.Application.Features.Email.Interfaces;
using dtc.Application.Features.Notifications.Interfaces;
using dtc.Application.Features.Training.Interfaces;
using dtc.Domain.Entities;
using dtc.Domain.Entities.Collaborators;
using dtc.Domain.Entities.Terms;
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

            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                // Upload documents with specific names for easy retrieval
                var uploadedDocs = new List<dtc.Domain.Entities.Permissions.Document>();
                if (request.Photo != null) uploadedDocs.Add(await UploadFileAsync(request.Photo, studentId, "image", "PROFILE_PHOTO"));
                if (request.IdFront != null) uploadedDocs.Add(await UploadFileAsync(request.IdFront, studentId, "image", "ID_FRONT"));
                if (request.IdBack != null) uploadedDocs.Add(await UploadFileAsync(request.IdBack, studentId, "image", "ID_BACK"));

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

                return await MapToDto(registration);
            });
        }

        private async Task<dtc.Domain.Entities.Permissions.Document> UploadFileAsync(IFormFile file, Guid studentId, string resourceType, string? customName = null)
        {
            var fileName = customName ?? file.FileName;
            using var stream = file.OpenReadStream();
            var (publicId, version) = await _cloudinaryService.UploadAsync(
                stream, 
                fileName, 
                $"user_docs/{studentId}", 
                resourceType);

            return new dtc.Domain.Entities.Permissions.Document(
                studentId,
                publicId,
                version,
                resourceType,
                fileName,
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
            var response = new List<CourseRegistrationResponseDto>();
            foreach (var reg in registrations)
            {
                response.Add(await MapToDto(reg));
            }
            return response;
        }

        public async Task<IEnumerable<CourseRegistrationResponseDto>> GetAllRegistrationsAsync()
        {
            var registrations = await _unitOfWork.CourseRegistrations.GetAllAsync();
            var response = new List<CourseRegistrationResponseDto>();
            foreach (var reg in registrations)
            {
                response.Add(await MapToDto(reg));
            }
            return response;
        }

        public async Task<CourseRegistrationResponseDto> GetRegistrationDetailAsync(Guid registrationId)
        {
            var registration = await _unitOfWork.CourseRegistrations.GetByIdAsync(registrationId);
            if (registration == null)
                throw new Exception("Registration not found");

            return await MapToDto(registration);
        }

        public async Task<object> GetRegistrationStatsAsync()
        {
            var now = DateTime.UtcNow;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            
            var allRegistrations = await _unitOfWork.CourseRegistrations.GetAllAsync();
            var list = allRegistrations.ToList();

            var newThisMonth = list.Count(r => r.RegistrationDate >= startOfMonth);
            var pendingCount = list.Count(r => r.Status == CourseRegistrationStatus.Pending);

            return new
            {
                NewRegistrationsThisMonth = newThisMonth,
                PendingRegistrations = pendingCount
            };
        }

        private async Task<CourseRegistrationResponseDto> MapToDto(CourseRegistration registration)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(registration.UserId);
            var course = await _unitOfWork.Courses.GetByIdAsync(registration.CourseId);
            var docs = await _unitOfWork.Documents.FindAsync(d => d.UserId == registration.UserId);
            var docList = docs.ToList();

            string? photoUrl = GetDocUrl(docList, "PROFILE_PHOTO", new[] { "photo", "avatar", "profile" });
            string? idFrontUrl = GetDocUrl(docList, "ID_FRONT", new[] { "front", "truoc" });
            string? idBackUrl = GetDocUrl(docList, "ID_BACK", new[] { "back", "sau" });

            return new CourseRegistrationResponseDto
            {
                Id = registration.Id,
                CourseId = registration.CourseId,
                UserId = registration.UserId,
                RegistrationDate = registration.RegistrationDate,
                Status = registration.Status.ToString(),
                TotalFee = registration.TotalFee,
                Notes = registration.Notes,
                CreatedAt = registration.CreatedAt,
                
                // User Info
                StudentName = user?.FullName ?? "N/A",
                Email = user?.Email.Value ?? "N/A",
                Phone = user?.Phone.Value ?? "N/A",

                // Course Info
                CourseName = course?.CourseName ?? "N/A",
                LicenseTypeLabel = course?.LicenseType.ToString() ?? "N/A",

                // Docs
                PhotoUrl = photoUrl,
                IdFrontUrl = idFrontUrl,
                IdBackUrl = idBackUrl
            };
        }

        private string? GetDocUrl(List<dtc.Domain.Entities.Permissions.Document> docs, string exactName, string[] keywords)
        {
            // 1. Try exact match (new naming convention)
            var doc = docs.OrderByDescending(d => d.CreatedAt).FirstOrDefault(d => d.FileName == exactName);
            
            // 2. Fallback to keyword search (for legacy or original filenames)
            if (doc == null)
            {
                doc = docs.OrderByDescending(d => d.CreatedAt)
                          .FirstOrDefault(d => keywords.Any(k => d.FileName.Contains(k, StringComparison.OrdinalIgnoreCase)));
            }

            // 3. Last resort for Photo: take first image found if it's the PROFILE_PHOTO request
            if (doc == null && exactName == "PROFILE_PHOTO")
            {
                doc = docs.OrderByDescending(d => d.CreatedAt)
                          .FirstOrDefault(d => d.ResourceType == "image");
            }

            if (doc == null) return null;
            return _cloudinaryService.GetUrl(doc.ProviderPublicId, doc.Version, doc.ResourceType);
        }
    }
}
