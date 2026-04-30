using dtc.Application.Features.Email.Interfaces;
using dtc.Application.Features.Notifications.Interfaces;
using dtc.Application.Features.Training.DTOs;
using dtc.Application.Features.Training.Interfaces;
using dtc.Application.Interfaces;
using dtc.Domain.Entities;
using dtc.Domain.Entities.Permissions;
using dtc.Domain.Entities.Classes;
using dtc.Domain.Entities.Collaborators;
using dtc.Domain.Entities.Terms;
using dtc.Domain.Interfaces;
using dtc.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dtc.Application.Features.Training.Services
{
    public class CourseRegistrationService : ICourseRegistrationService
    {
        private const decimal ReferralDiscountRate = 0.05m;
        private const decimal CollaboratorCommissionRate = 0.05m;
        private const int TermPlacementGraceDays = 7;

        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;
        private readonly IEmailService _emailService;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly ILogger<CourseRegistrationService> _logger;

        public CourseRegistrationService(
            IUnitOfWork unitOfWork,
            INotificationService notificationService,
            IEmailService emailService,
            ICloudinaryService cloudinaryService,
            ILogger<CourseRegistrationService> logger)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            _emailService = emailService;
            _cloudinaryService = cloudinaryService;
            _logger = logger;
        }

        public async Task<CourseRegistrationResponseDto> RegisterCourseAsync(RegisterCourseRequestDto request, Guid? studentId)
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId);
            if (course == null || !course.IsActive)
                throw new Exception("Course not found or inactive");

            var center = await _unitOfWork.Centers.GetByIdAsync(course.CenterId);
            var referralCode = await ResolveReferralCodeAsync(request.ReferralCode, course.CenterId);

            var submission = await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var student = await ResolveStudentForRegistrationAsync(request, studentId);
                var resolvedStudentId = student.Id;
                var originalFee = course.Price;
                var discountAmount = referralCode == null
                    ? 0m
                    : Math.Round(originalFee * ReferralDiscountRate, 2, MidpointRounding.AwayFromZero);
                var finalFee = Math.Max(0, Math.Round(originalFee - discountAmount, 2, MidpointRounding.AwayFromZero));

                var uploadedDocs = new List<dtc.Domain.Entities.Permissions.Document>();
                if (request.Photo != null) uploadedDocs.Add(await UploadFileAsync(request.Photo, resolvedStudentId, "image", "PROFILE_PHOTO"));
                if (request.IdFront != null) uploadedDocs.Add(await UploadFileAsync(request.IdFront, resolvedStudentId, "image", "ID_FRONT"));
                if (request.IdBack != null) uploadedDocs.Add(await UploadFileAsync(request.IdBack, resolvedStudentId, "image", "ID_BACK"));

                foreach (var doc in uploadedDocs)
                {
                    await _unitOfWork.Documents.AddAsync(doc);
                }

                var registration = new CourseRegistration(
                    request.CourseId,
                    resolvedStudentId,
                    finalFee,
                    request.Notes,
                    resolvedStudentId,
                    originalFee);

                await _unitOfWork.CourseRegistrations.AddAsync(registration);
                
                // Assign the student to the center of the course if not already assigned
                var existingLink = await _unitOfWork.UserCenters.FindAsync(uc => uc.UserId == resolvedStudentId && uc.CenterId == course.CenterId);
                if (!existingLink.Any())
                {
                    await _unitOfWork.UserCenters.AddAsync(new dtc.Domain.Entities.Permissions.UserCenter(resolvedStudentId, course.CenterId));
                }

                await _unitOfWork.SaveChangesAsync();

                if (referralCode != null)
                {
                    var referralRegistration = new ReferralRegistration(referralCode.Id, resolvedStudentId, registration.Id);
                    await _unitOfWork.ReferralRegistrations.AddAsync(referralRegistration);

                    var commissionAmount = Math.Round(originalFee * CollaboratorCommissionRate, 2, MidpointRounding.AwayFromZero);
                    var collaboratorCommission = new CollaboratorCommission(
                        referralCode.CollaboratorId,
                        commissionAmount,
                        referralRegistration.Id);
                    await _unitOfWork.CollaboratorCommissions.AddAsync(collaboratorCommission);

                    referralCode.IncreaseUsage(resolvedStudentId);
                    await _unitOfWork.ReferralCodes.UpdateAsync(referralCode);

                    var studentName = student.FullName ?? "Học viên mới";

                    await _notificationService.CreateForUserAsync(
                        referralCode.CollaboratorId,
                        "Mã giới thiệu được sử dụng",
                        $"{studentName} đã sử dụng mã '{referralCode.Code}' để đăng ký '{course.CourseName}'. Hoa hồng tạm tính: {commissionAmount:N0} VND.",
                        NotificationType.Referral);
                }

                return new RegistrationSubmissionResult
                {
                    Response = await MapToDto(registration),
                    StudentId = student.Id,
                    StudentName = student.FullName ?? "Học viên",
                    StudentEmail = student.Email.Value,
                    CourseName = course.CourseName,
                    CenterName = center?.CenterName ?? "DTC"
                };
            });

            await NotifyRegistrationReceivedAsync(submission);
            return submission.Response;
        }

        private async Task<User> ResolveStudentForRegistrationAsync(RegisterCourseRequestDto request, Guid? authenticatedStudentId)
        {
            if (authenticatedStudentId.HasValue && authenticatedStudentId.Value != Guid.Empty)
            {
                var signedInUser = await _unitOfWork.Users.GetByIdAsync(authenticatedStudentId.Value);
                if (signedInUser == null)
                {
                    _logger.LogWarning(
                        "Authenticated course registration fallback activated because local user {UserId} was not found.",
                        authenticatedStudentId.Value);
                }
                else
                {
                    return signedInUser;
                }
            }

            if (string.IsNullOrWhiteSpace(request.Email))
            {
                throw new InvalidOperationException("Email là bắt buộc để đăng ký khóa học.");
            }

            var email = dtc.Domain.ValueObjects.Email.Create(request.Email);
            var existingUser = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (existingUser != null)
            {
                if (existingUser.RoleId != UserRole.Student)
                {
                    throw new InvalidOperationException("Email này đang thuộc về một tài khoản nội bộ khác. Vui lòng đăng nhập đúng tài khoản trước khi đăng ký.");
                }

                if (!string.IsNullOrWhiteSpace(request.FullName) || !string.IsNullOrWhiteSpace(request.Phone))
                {
                    var resolvedFullName = !string.IsNullOrWhiteSpace(request.FullName)
                        ? request.FullName.Trim()
                        : existingUser.FullName;
                    var resolvedPhone = !string.IsNullOrWhiteSpace(request.Phone)
                        ? dtc.Domain.ValueObjects.PhoneNumber.Create(request.Phone)
                        : existingUser.Phone ?? dtc.Domain.ValueObjects.PhoneNumber.Create("0000000000");

                    existingUser.UpdateProfile(resolvedFullName, resolvedPhone, existingUser.AvatarUrl, existingUser.Id);
                }

                existingUser.Activate(existingUser.Id);
                return existingUser!;
            }

            if (string.IsNullOrWhiteSpace(request.FullName) || string.IsNullOrWhiteSpace(request.Phone))
            {
                throw new InvalidOperationException("Họ tên, email và số điện thoại là bắt buộc khi đăng ký tài khoản mới.");
            }

            var phone = dtc.Domain.ValueObjects.PhoneNumber.Create(request.Phone);

            var pendingClerkId = $"pending_local_{Guid.NewGuid():N}";
            var newUser = new User(
                clerkId: pendingClerkId,
                email: email,
                fullName: request.FullName.Trim(),
                phone: phone,
                roleId: UserRole.Student,
                createdBy: null);

            await _unitOfWork.Users.AddAsync(newUser);
            return newUser;
        }

        public async Task CancelRegistrationAsync(Guid registrationId, string reason, Guid studentId)
        {
            var registration = await _unitOfWork.CourseRegistrations.GetByIdAsync(registrationId);
            if (registration == null)
                throw new Exception("Registration not found");

            if (registration.UserId != studentId)
                throw new Exception("Unauthorized to cancel this registration");

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                if (registration.Status == CourseRegistrationStatus.Approved)
                {
                    await ReleasePlacementAsync(registration, studentId);
                }

                registration.Cancel(reason, studentId);
                await _unitOfWork.CourseRegistrations.UpdateAsync(registration);
            });
        }

        public async Task UpdateRegistrationStatusAsync(Guid registrationId, UpdateRegistrationStatusDto request, Guid adminId)
        {
            PlacementAssignment? placement = null;
            var registration = await _unitOfWork.CourseRegistrations.GetByIdAsync(registrationId);
            if (registration == null)
                throw new Exception("Registration not found");

            if (request.Status == CourseRegistrationStatus.Approved)
            {
                placement = await _unitOfWork.ExecuteInTransactionAsync(async () =>
                {
                    var trackedRegistration = await _unitOfWork.CourseRegistrations.GetByIdAsync(registrationId)
                        ?? throw new Exception("Registration not found");

                    var assignedPlacement = await AssignRegistrationToPlacementAsync(trackedRegistration, adminId);
                    trackedRegistration.Approve(adminId);

                    await _unitOfWork.CourseRegistrations.UpdateAsync(trackedRegistration);
                    return assignedPlacement;
                });
            }
            else if (request.Status == CourseRegistrationStatus.Rejected)
            {
                registration.Reject(request.Reason, adminId);
                await _unitOfWork.CourseRegistrations.UpdateAsync(registration);
                await _unitOfWork.SaveChangesAsync();
            }
            else if (request.Status == CourseRegistrationStatus.Cancelled)
            {
                await _unitOfWork.ExecuteInTransactionAsync(async () =>
                {
                    var trackedRegistration = await _unitOfWork.CourseRegistrations.GetByIdAsync(registrationId)
                        ?? throw new Exception("Registration not found");

                    if (trackedRegistration.Status == CourseRegistrationStatus.Approved)
                    {
                        await ReleasePlacementAsync(trackedRegistration, adminId);
                    }

                    trackedRegistration.Cancel(request.Reason, adminId);
                    await _unitOfWork.CourseRegistrations.UpdateAsync(trackedRegistration);
                });
            }

            await NotifyRegistrationStatusChangedAsync(registrationId, request.Status, request.Reason, placement);
        }

        public async Task<IEnumerable<CourseRegistrationResponseDto>> GetMyRegistrationsAsync(Guid studentId)
        {
            var registrations = (await _unitOfWork.CourseRegistrations.FindAsync(r => r.UserId == studentId))
                .OrderByDescending(r => r.RegistrationDate)
                .ToList();
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

        public async Task<CourseRegistrationPagedResponseDto> GetRegistrationsPagedAsync(
            CourseRegistrationPagedQueryDto query,
            Guid? managedCenterId = null)
        {
            var pageNumber = Math.Max(1, query.PageNumber);
            var pageSize = Math.Max(1, query.PageSize);
            var search = query.Search?.Trim();
            var normalizedLicense = string.IsNullOrWhiteSpace(query.LicenseType) || string.Equals(query.LicenseType, "ALL", StringComparison.OrdinalIgnoreCase)
                ? null
                : query.LicenseType.Trim();
            var normalizedStatus = string.IsNullOrWhiteSpace(query.Status) || string.Equals(query.Status, "ALL", StringComparison.OrdinalIgnoreCase)
                ? null
                : query.Status.Trim();

            List<Guid>? managedCourseIds = null;
            if (managedCenterId.HasValue)
            {
                managedCourseIds = (await _unitOfWork.Courses.FindAsync(c => c.CenterId == managedCenterId.Value && !c.IsDeleted))
                    .Select(c => c.Id)
                    .Distinct()
                    .ToList();

                if (managedCourseIds.Count == 0)
                {
                    return new CourseRegistrationPagedResponseDto
                    {
                        PageNumber = pageNumber,
                        PageSize = pageSize,
                        TotalItems = 0,
                        TotalPages = 0,
                        NewRegistrationsThisMonth = 0,
                        PendingRegistrations = 0,
                        Items = []
                    };
                }
            }

            var registrations = managedCourseIds != null
                ? (await _unitOfWork.CourseRegistrations.FindAsync(r => managedCourseIds.Contains(r.CourseId))).ToList()
                : (await _unitOfWork.CourseRegistrations.GetAllAsync()).ToList();

            var now = DateTime.UtcNow;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var newRegistrationsThisMonth = registrations.Count(r => r.RegistrationDate >= startOfMonth);
            var pendingRegistrations = registrations.Count(r => r.Status == CourseRegistrationStatus.Pending);

            if (!string.IsNullOrWhiteSpace(normalizedStatus) &&
                Enum.TryParse<CourseRegistrationStatus>(normalizedStatus, true, out var statusFilter))
            {
                registrations = registrations
                    .Where(r => r.Status == statusFilter)
                    .ToList();
            }

            var courseIds = registrations.Select(r => r.CourseId).Distinct().ToList();
            var userIds = registrations.Select(r => r.UserId).Distinct().ToList();

            var courses = courseIds.Count == 0
                ? []
                : (await _unitOfWork.Courses.FindAsync(c => courseIds.Contains(c.Id))).ToList();
            var users = userIds.Count == 0
                ? []
                : (await _unitOfWork.Users.FindAsync(u => userIds.Contains(u.Id))).ToList();

            var courseLookup = courses.ToDictionary(c => c.Id);
            var userLookup = users.ToDictionary(u => u.Id);

            if (!string.IsNullOrWhiteSpace(normalizedLicense))
            {
                registrations = registrations
                    .Where(r =>
                        courseLookup.TryGetValue(r.CourseId, out var course) &&
                        string.Equals(course.LicenseType.ToString(), normalizedLicense, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var normalizedSearch = search.ToLowerInvariant();
                registrations = registrations
                    .Where(r =>
                    {
                        courseLookup.TryGetValue(r.CourseId, out var course);
                        userLookup.TryGetValue(r.UserId, out var user);

                        return (user?.FullName?.ToLowerInvariant().Contains(normalizedSearch) ?? false)
                               || (user?.Email.Value?.ToLowerInvariant().Contains(normalizedSearch) ?? false)
                               || (user?.Phone?.Value?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false)
                               || r.UserId.ToString().Contains(search, StringComparison.OrdinalIgnoreCase)
                               || (course?.CourseName?.ToLowerInvariant().Contains(normalizedSearch) ?? false);
                    })
                    .ToList();
            }

            registrations = registrations
                .OrderByDescending(r => r.RegistrationDate)
                .ThenByDescending(r => r.CreatedAt)
                .ToList();

            var totalItems = registrations.Count;
            var pagedRegistrations = registrations
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var items = new List<CourseRegistrationResponseDto>();
            foreach (var registration in pagedRegistrations)
            {
                items.Add(await MapToDto(registration));
            }

            return new CourseRegistrationPagedResponseDto
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalItems == 0 ? 0 : (int)Math.Ceiling((double)totalItems / pageSize),
                NewRegistrationsThisMonth = newRegistrationsThisMonth,
                PendingRegistrations = pendingRegistrations,
                Items = items
            };
        }

        public async Task<CourseRegistrationResponseDto> GetRegistrationDetailAsync(Guid registrationId)
        {
            var registration = await _unitOfWork.CourseRegistrations.GetByIdAsync(registrationId);
            if (registration == null)
                throw new Exception("Registration not found");

            return await MapToDto(registration);
        }

        public async Task<IReadOnlyCollection<CourseRegistrationTermOptionDto>> GetRegistrationTermOptionsAsync(Guid registrationId)
        {
            var registration = await _unitOfWork.CourseRegistrations.GetByIdAsync(registrationId);
            if (registration == null)
                throw new Exception("Registration not found");

            var effectiveDate = GetPlacementReferenceDate(registration.RegistrationDate);
            var today = DateTime.UtcNow.Date;

            var terms = (await _unitOfWork.Terms.FindAsync(t =>
                    t.CourseId == registration.CourseId
                    && !t.IsDeleted
                    && t.IsActive
                    && t.EndDate >= today))
                .OrderBy(t => t.StartDate)
                .ToList();

            return terms
                .Where(t => t.Id == registration.AssignedTermId || t.CurrentStudents < t.MaxStudents)
                .Select(t => new CourseRegistrationTermOptionDto
                {
                    Id = t.Id,
                    TermName = t.TermName,
                    StartDate = t.StartDate,
                    EndDate = t.EndDate,
                    CurrentStudents = t.CurrentStudents,
                    MaxStudents = t.MaxStudents,
                    IsCurrentAssignment = registration.AssignedTermId == t.Id,
                    IsLateForAutoPlacement = IsLateForAutomaticPlacement(t, effectiveDate)
                })
                .ToList();
        }

        public async Task<CourseRegistrationResponseDto> ReassignRegistrationTermAsync(Guid registrationId, Guid termId, Guid adminId)
        {
            var response = await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var registration = await _unitOfWork.CourseRegistrations.GetByIdAsync(registrationId)
                    ?? throw new Exception("Registration not found");

                if (registration.Status != CourseRegistrationStatus.Approved
                    && registration.Status != CourseRegistrationStatus.Pending)
                {
                    throw new InvalidOperationException("Chỉ có thể chuyển kỳ thủ công cho hồ sơ đang chờ duyệt hoặc đã duyệt.");
                }

                var targetTerm = await _unitOfWork.Terms.GetByIdAsync(termId)
                    ?? throw new InvalidOperationException("Không tìm thấy kỳ học được chọn.");

                if (targetTerm.CourseId != registration.CourseId)
                {
                    throw new InvalidOperationException("Chỉ được chuyển sang kỳ học thuộc cùng khóa học.");
                }

                if (targetTerm.IsDeleted || !targetTerm.IsActive || targetTerm.EndDate.Date < DateTime.UtcNow.Date)
                {
                    throw new InvalidOperationException("Kỳ học được chọn không còn khả dụng.");
                }

                if (registration.AssignedTermId == targetTerm.Id)
                {
                    return await MapToDto(registration);
                }

                if (targetTerm.CurrentStudents >= targetTerm.MaxStudents)
                {
                    throw new InvalidOperationException("Kỳ học được chọn đã đủ sĩ số.");
                }

                await ReleasePlacementAsync(registration, adminId);

                targetTerm.EnrollStudent(adminId);
                registration.AssignTerm(targetTerm.Id, adminId);

                await _unitOfWork.Terms.UpdateAsync(targetTerm);
                await _unitOfWork.CourseRegistrations.UpdateAsync(registration);

                return await MapToDto(registration);
            });

            await NotifyRegistrationTermReassignedAsync(response);
            return response;
        }

        public async Task<object> GetRegistrationStatsAsync()
        {
            var now = DateTime.UtcNow;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);

            var allRegistrations = await _unitOfWork.CourseRegistrations.GetAllAsync();
            var list = allRegistrations.ToList();

            return new
            {
                NewRegistrationsThisMonth = list.Count(r => r.RegistrationDate >= startOfMonth),
                PendingRegistrations = list.Count(r => r.Status == CourseRegistrationStatus.Pending)
            };
        }

        private async Task<PlacementAssignment> AssignRegistrationToPlacementAsync(CourseRegistration registration, Guid adminId)
        {
            var term = await FindAvailableTermAsync(registration.CourseId, GetPlacementReferenceDate(registration.RegistrationDate));
            if (term == null)
            {
                throw new InvalidOperationException("Không tìm thấy kỳ học phù hợp còn chỗ trống cho khóa học này. Vui lòng tạo thêm kỳ học hoặc tăng sĩ số trước khi duyệt.");
            }

            term.EnrollStudent(adminId);
            registration.AssignTerm(term.Id, adminId);
            await _unitOfWork.Terms.UpdateAsync(term);

            return new PlacementAssignment
            {
                Term = term,
                AssignedClass = null
            };
        }

        private async Task ReleasePlacementAsync(CourseRegistration registration, Guid updatedBy)
        {
            if (registration.AssignedTermId.HasValue)
            {
                var term = await _unitOfWork.Terms.GetByIdAsync(registration.AssignedTermId.Value);
                if (term != null)
                {
                    term.RemoveStudent(updatedBy);
                    await _unitOfWork.Terms.UpdateAsync(term);
                }

                var termClasses = (await _unitOfWork.Classes.FindAsync(c => c.TermId == registration.AssignedTermId.Value && !c.IsDeleted)).ToList();
                if (termClasses.Count > 0)
                {
                    var classIds = termClasses.Select(c => c.Id).ToList();
                    var enrollments = (await _unitOfWork.ClassStudents.FindAsync(cs => classIds.Contains(cs.ClassId) && cs.StudentId == registration.UserId)).ToList();

                    if (enrollments.Count > 0)
                    {
                        await _unitOfWork.ClassStudents.RemoveRange(enrollments);

                        foreach (var classEntity in termClasses.Where(c => enrollments.Any(e => e.ClassId == c.Id)))
                        {
                            classEntity.RemoveStudent(updatedBy);
                            await _unitOfWork.Classes.UpdateAsync(classEntity);
                        }
                    }
                }

                registration.ClearAssignedTerm(updatedBy);
            }
        }

        private async Task<Class?> TryAssignStudentToClassAsync(Guid termId, Guid studentId, Guid adminId)
        {
            var availableClasses = (await _unitOfWork.Classes.FindAsync(c =>
                    c.TermId == termId
                    && !c.IsDeleted
                    && (c.Status == ClassStatus.Pending || c.Status == ClassStatus.InProgress)))
                .Where(c => c.CurrentStudents < c.MaxStudents)
                .OrderBy(c => c.ClassType == ClassType.Theory ? 0 : 1)
                .ThenBy(c => c.CurrentStudents)
                .ThenBy(c => c.CreatedAt)
                .ToList();

            var selectedClass = availableClasses.FirstOrDefault();
            if (selectedClass == null)
            {
                return null;
            }

            var existingEnrollment = await _unitOfWork.ClassStudents.FindAsync(cs => cs.ClassId == selectedClass.Id && cs.StudentId == studentId);
            if (!existingEnrollment.Any())
            {
                selectedClass.EnrollStudent(adminId);
                await _unitOfWork.ClassStudents.AddAsync(new ClassStudent(selectedClass.Id, studentId));
                await _unitOfWork.Classes.UpdateAsync(selectedClass);
            }

            return selectedClass;
        }

        private async Task<Term?> FindAvailableTermAsync(Guid courseId, DateTime registrationDate)
        {
            var effectiveDate = GetPlacementReferenceDate(registrationDate);
            var candidateTerms = (await _unitOfWork.Terms.FindAsync(t =>
                    t.CourseId == courseId
                    && !t.IsDeleted
                    && t.IsActive
                    && t.EndDate >= effectiveDate))
                .Where(t => !IsLateForAutomaticPlacement(t, effectiveDate))
                .OrderBy(t => GetTermPriority(t, effectiveDate))
                .ThenBy(t => t.StartDate)
                .ToList();

            return candidateTerms.FirstOrDefault(t => t.CurrentStudents < t.MaxStudents);
        }

        private async Task<Term?> FindSuggestedTermAsync(Guid courseId, DateTime registrationDate)
        {
            return await FindAvailableTermAsync(courseId, registrationDate);
        }

        private static int GetTermPriority(Term term, DateTime registrationDate)
        {
            if (term.StartDate <= registrationDate && term.EndDate >= registrationDate)
            {
                return 0;
            }

            return term.StartDate > registrationDate ? 1 : 2;
        }

        private static DateTime GetPlacementReferenceDate(DateTime registrationDate)
        {
            var now = DateTime.UtcNow;
            var effectiveDate = registrationDate > now ? registrationDate : now;
            return effectiveDate.Date;
        }

        private static bool IsLateForAutomaticPlacement(Term term, DateTime effectiveDate)
        {
            if (effectiveDate.Date < term.StartDate.Date)
            {
                return false;
            }

            var autoPlacementDeadline = term.StartDate.Date.AddDays(TermPlacementGraceDays);
            return effectiveDate.Date > autoPlacementDeadline;
        }

        private async Task NotifyRegistrationReceivedAsync(RegistrationSubmissionResult submission)
        {
            try
            {
                var message = !string.IsNullOrWhiteSpace(submission.Response.PlacementMessage)
                    ? $"Hồ sơ đăng ký '{submission.CourseName}' đã được ghi nhận. {submission.Response.PlacementMessage}"
                    : $"Hồ sơ đăng ký '{submission.CourseName}' đã được ghi nhận. Trung tâm sẽ liên hệ sớm để xác nhận lịch học phù hợp.";

                await _notificationService.CreateForUserAsync(
                    submission.StudentId,
                    "Đã tiếp nhận hồ sơ đăng ký",
                    message,
                    NotificationType.Registration);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "Failed to create registration notification for student {StudentId}, course {CourseName}.",
                    submission.StudentId,
                    submission.CourseName);
            }

            try
            {
                await _emailService.SendCourseRegistrationConfirmationAsync(
                    submission.StudentEmail,
                    submission.StudentName,
                    submission.CourseName,
                    submission.CenterName);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to send course registration confirmation email to {Email} for course {CourseName}.",
                    submission.StudentEmail,
                    submission.CourseName);
            }

            try
            {
                await _emailService.SendCourseRegistrationConfirmationAsync(
                    submission.StudentEmail,
                    submission.StudentName,
                    submission.CourseName,
                    submission.CenterName);
            }
            catch
            {
            }
        }

        private async Task<ReferralCode?> ResolveReferralCodeAsync(string? rawReferralCode, Guid courseCenterId)
        {
            if (string.IsNullOrWhiteSpace(rawReferralCode))
            {
                return null;
            }

            var normalizedCode = rawReferralCode.Trim().ToUpperInvariant();
            var referralCode = (await _unitOfWork.ReferralCodes.FindAsync(c => c.Code == normalizedCode && c.IsActive))
                .FirstOrDefault();

            if (referralCode == null)
            {
                throw new InvalidOperationException("Referral code is invalid or inactive.");
            }

            var collaboratorCenter = (await _unitOfWork.UserCenters.FindAsync(uc => uc.UserId == referralCode.CollaboratorId))
                .FirstOrDefault();
            if (collaboratorCenter == null || collaboratorCenter.CenterId != courseCenterId)
            {
                throw new InvalidOperationException("Referral code does not apply to this training center.");
            }

            return referralCode;
        }

        private async Task NotifyRegistrationStatusChangedAsync(
            Guid registrationId,
            CourseRegistrationStatus newStatus,
            string reason,
            PlacementAssignment? placement)
        {
            try
            {
                var registration = await _unitOfWork.CourseRegistrations.GetByIdAsync(registrationId);
                if (registration == null) return;

                var student = await _unitOfWork.Users.GetByIdAsync(registration.UserId);
                var course = await _unitOfWork.Courses.GetByIdAsync(registration.CourseId);
                if (student == null || course == null) return;

                string title;
                string content;

                if (newStatus == CourseRegistrationStatus.Approved)
                {
                    if (placement?.AssignedClass != null)
                    {
                        content = $"Bạn đã được chấp nhận vào khóa học '{course.CourseName}', xếp vào kỳ '{placement.Term.TermName}' và lớp '{placement.AssignedClass.ClassName}'.";
                    }
                    else if (placement != null)
                    {
                        content = $"Bạn đã được chấp nhận vào khóa học '{course.CourseName}' và xếp vào kỳ '{placement.Term.TermName}'. Trung tâm sẽ bố trí lớp phù hợp trong kỳ này cho bạn sớm nhất.";
                    }
                    else
                    {
                        content = $"Bạn đã được chấp nhận vào khóa học '{course.CourseName}'.";
                    }

                    title = "Đăng ký khóa học thành công";

                    var center = await _unitOfWork.Centers.GetByIdAsync(course.CenterId);
                    await _emailService.SendCourseRegistrationConfirmationAsync(
                        student.Email.Value,
                        student.FullName,
                        course.CourseName,
                        center?.CenterName ?? "DTC");
                }
                else if (newStatus == CourseRegistrationStatus.Rejected)
                {
                    title = "Từ chối hồ sơ đăng ký";
                    content = $"Hồ sơ đăng ký khóa học '{course.CourseName}' của bạn đã bị từ chối. Lý do: {reason}";
                }           
                else
                {
                    title = "Hủy hồ sơ đăng ký";
                    content = $"Hồ sơ đăng ký khóa học '{course.CourseName}' đã bị hủy. Lý do: {reason}";
                }

                await _notificationService.CreateForUserAsync(student.Id, title, content, NotificationType.Registration);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to notify registration status change for registration {RegistrationId} with status {Status}.",
                    registrationId,
                    newStatus);
            }
        }

        private async Task NotifyRegistrationTermReassignedAsync(CourseRegistrationResponseDto registration)
        {
            Term? assignedTerm = null;
            if (registration.AssignedTermId.HasValue)
            {
                assignedTerm = await _unitOfWork.Terms.GetByIdAsync(registration.AssignedTermId.Value);
            }

            var termDateRangeText = assignedTerm == null
                ? null
                : $"{assignedTerm.StartDate:dd/MM/yyyy} - {assignedTerm.EndDate:dd/MM/yyyy}";

            try
            {
                var courseName = string.IsNullOrWhiteSpace(registration.CourseName) ? "khóa học đã đăng ký" : registration.CourseName!;
                var assignedTermName = string.IsNullOrWhiteSpace(registration.AssignedTermName)
                    ? "kỳ học mới"
                    : registration.AssignedTermName!;
                var notificationContent = $"Hồ sơ đăng ký '{courseName}' của bạn đã được trung tâm chuyển sang kỳ '{assignedTermName}'";
                if (!string.IsNullOrWhiteSpace(termDateRangeText))
                {
                    notificationContent += $" ({termDateRangeText})";
                }

                notificationContent += ". Vui lòng theo dõi thông báo tiếp theo để biết thông tin xếp lớp.";

                await _notificationService.CreateForUserAsync(
                    registration.UserId,
                    "Đã cập nhật kỳ học đăng ký",
                    notificationContent,
                    NotificationType.Registration);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "Failed to create term reassignment notification for registration {RegistrationId}.",
                    registration.Id);
            }

            try
            {
                var courseName = string.IsNullOrWhiteSpace(registration.CourseName) ? "khóa học đã đăng ký" : registration.CourseName!;
                var assignedTermName = string.IsNullOrWhiteSpace(registration.AssignedTermName)
                    ? "kỳ học mới"
                    : registration.AssignedTermName!;
                var studentName = string.IsNullOrWhiteSpace(registration.StudentName) ? "Học viên" : registration.StudentName;
                var centerName = string.IsNullOrWhiteSpace(registration.CenterName) ? "Drive Safe Academy" : registration.CenterName!;
                var statusLabel = registration.Status switch
                {
                    "Approved" => "Đã duyệt",
                    "Pending" => "Chờ duyệt",
                    "Rejected" => "Từ chối",
                    "Cancelled" => "Đã hủy",
                    _ => registration.Status
                };
                var body = $@"
<h2>Cập nhật kỳ học đăng ký</h2>
<p>Xin chào <b>{System.Web.HttpUtility.HtmlEncode(studentName)}</b>,</p>
<p>Trung tâm vừa cập nhật hồ sơ đăng ký khóa học <b>{System.Web.HttpUtility.HtmlEncode(courseName)}</b> của bạn.</p>
<p><b>Kỳ học mới:</b> {System.Web.HttpUtility.HtmlEncode(assignedTermName)}</p>
{(assignedTerm == null ? string.Empty : $"<p><b>Ngày bắt đầu kỳ học:</b> {assignedTerm.StartDate:dd/MM/yyyy}</p><p><b>Ngày kết thúc kỳ học:</b> {assignedTerm.EndDate:dd/MM/yyyy}</p>")}
<p>Hiện tại hồ sơ của bạn đang ở trạng thái <b>{statusLabel}</b>. Trung tâm sẽ tiếp tục gửi thông báo khi có lịch xếp lớp cụ thể.</p>
<hr/>
<p style=""font-size:12px;color:gray;"">Email được gửi từ {System.Web.HttpUtility.HtmlEncode(centerName)}.</p>";

                await _emailService.SendAsync(new dtc.Application.Features.Email.DTOs.SendEmailRequestDto
                {
                    ToEmail = registration.Email,
                    ToName = studentName,
                    Subject = $"Cập nhật kỳ học - {courseName}",
                    Body = body,
                    IsHtml = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "Failed to send term reassignment email for registration {RegistrationId}.",
                    registration.Id);
            }
        }

        private async Task<CourseRegistrationResponseDto> MapToDto(CourseRegistration registration)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(registration.UserId);
            var course = await _unitOfWork.Courses.GetByIdAsync(registration.CourseId);
            var docs = await _unitOfWork.Documents.FindAsync(d => d.UserId == registration.UserId);
            var docList = docs.ToList();

            Term? assignedTerm = null;
            if (registration.AssignedTermId.HasValue)
            {
                assignedTerm = await _unitOfWork.Terms.GetByIdAsync(registration.AssignedTermId.Value);
            }

            Class? assignedClass = null;
            if (assignedTerm != null)
            {
                var termClasses = (await _unitOfWork.Classes.FindAsync(c => c.TermId == assignedTerm.Id && !c.IsDeleted)).ToList();
                if (termClasses.Count > 0)
                {
                    var classIds = termClasses.Select(c => c.Id).ToList();
                    var classStudent = (await _unitOfWork.ClassStudents.FindAsync(cs => classIds.Contains(cs.ClassId) && cs.StudentId == registration.UserId)).FirstOrDefault();
                    if (classStudent != null)
                    {
                        assignedClass = termClasses.FirstOrDefault(c => c.Id == classStudent.ClassId);
                    }
                }
            }

            var suggestedTerm = registration.Status == CourseRegistrationStatus.Pending
                ? await FindSuggestedTermAsync(registration.CourseId, registration.RegistrationDate)
                : null;

            return new CourseRegistrationResponseDto
            {
                Id = registration.Id,
                CourseId = registration.CourseId,
                UserId = registration.UserId,
                RegistrationDate = registration.RegistrationDate,
                Status = registration.Status.ToString(),
                TotalFee = registration.TotalFee,
                OriginalFee = registration.OriginalFee,
                DiscountAmount = Math.Max(0, registration.OriginalFee - registration.TotalFee),
                Notes = registration.Notes,
                CreatedAt = registration.CreatedAt,
                StudentName = user?.FullName ?? "N/A",
                Email = user?.Email.Value ?? "N/A",
                Phone = user?.Phone?.Value ?? "N/A",
                CenterId = course?.CenterId,
                CenterName = course != null
                    ? (await _unitOfWork.Centers.GetByIdAsync(course.CenterId))?.CenterName
                    : null,
                CourseName = course?.CourseName ?? "N/A",
                LicenseTypeLabel = course?.LicenseType.ToString() ?? "N/A",
                AssignedTermId = registration.AssignedTermId,
                AssignedTermName = assignedTerm?.TermName,
                AssignedClassId = assignedClass?.Id,
                AssignedClassName = assignedClass?.ClassName,
                SuggestedTermId = suggestedTerm?.Id,
                SuggestedTermName = suggestedTerm?.TermName,
                SuggestedTermStartDate = suggestedTerm?.StartDate,
                PlacementMessage = BuildPlacementMessage(registration, assignedTerm, assignedClass, suggestedTerm),
                AppliedReferralCode = await GetAppliedReferralCodeAsync(registration.Id),
                AppliedReferralCollaboratorName = await GetAppliedReferralCollaboratorNameAsync(registration.Id),
                PhotoUrl = GetDocUrl(docList, "PROFILE_PHOTO", new[] { "photo", "avatar", "profile" }),
                IdFrontUrl = GetDocUrl(docList, "ID_FRONT", new[] { "front", "truoc" }),
                IdBackUrl = GetDocUrl(docList, "ID_BACK", new[] { "back", "sau" })
            };
        }

        private async Task<string?> GetAppliedReferralCodeAsync(Guid registrationId)
        {
            var referralRegistration = (await _unitOfWork.ReferralRegistrations.FindAsync(r => r.CourseRegistrationId == registrationId))
                .FirstOrDefault();
            if (referralRegistration == null)
            {
                return null;
            }

            var referralCode = await _unitOfWork.ReferralCodes.GetByIdAsync(referralRegistration.ReferralCodeId);
            return referralCode?.Code;
        }

        private async Task<string?> GetAppliedReferralCollaboratorNameAsync(Guid registrationId)
        {
            var referralRegistration = (await _unitOfWork.ReferralRegistrations.FindAsync(r => r.CourseRegistrationId == registrationId))
                .FirstOrDefault();
            if (referralRegistration == null)
            {
                return null;
            }

            var referralCode = await _unitOfWork.ReferralCodes.GetByIdAsync(referralRegistration.ReferralCodeId);
            if (referralCode == null)
            {
                return null;
            }

            var collaborator = await _unitOfWork.Users.GetByIdAsync(referralCode.CollaboratorId);
            return collaborator?.FullName;
        }

        private static string? BuildPlacementMessage(
            CourseRegistration registration,
            Term? assignedTerm,
            Class? assignedClass,
            Term? suggestedTerm)
        {
            if (registration.Status == CourseRegistrationStatus.Approved && assignedTerm != null)
            {
                return assignedClass != null
                    ? $"Đã xếp vào kỳ '{assignedTerm.TermName}' và lớp '{assignedClass.ClassName}'."
                    : $"Đã xếp vào kỳ '{assignedTerm.TermName}'. Trung tâm sẽ xếp lớp lý thuyết và thực hành phù hợp sau.";
            }

            if (registration.Status == CourseRegistrationStatus.Pending && suggestedTerm != null)
            {
                return $"Dự kiến xếp vào kỳ '{suggestedTerm.TermName}' bắt đầu {suggestedTerm.StartDate:dd/MM/yyyy} nếu còn chỗ.";
            }

            if (registration.Status == CourseRegistrationStatus.Pending)
            {
                return $"Chưa tìm thấy kỳ học còn chỗ trống hoặc còn trong thời gian nhận học viên (tối đa {TermPlacementGraceDays} ngày đầu kỳ).";
            }

            return null;
        }

        private async Task<dtc.Domain.Entities.Permissions.Document> UploadFileAsync(UploadedFileDto file, Guid studentId, string resourceType, string? customName = null)
        {
            var fileName = customName ?? file.FileName;
            using var stream = new System.IO.MemoryStream(file.Content);
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
                file.Extension,
                file.Length);
        }

        private string? GetDocUrl(List<dtc.Domain.Entities.Permissions.Document> docs, string exactName, string[] keywords)
        {
            var doc = docs.OrderByDescending(d => d.CreatedAt).FirstOrDefault(d => d.FileName == exactName);
            if (doc == null)
            {
                doc = docs.OrderByDescending(d => d.CreatedAt)
                    .FirstOrDefault(d => keywords.Any(k => d.FileName.Contains(k, StringComparison.OrdinalIgnoreCase)));
            }

            if (doc == null && exactName == "PROFILE_PHOTO")
            {
                doc = docs.OrderByDescending(d => d.CreatedAt)
                    .FirstOrDefault(d => d.ResourceType == "image");
            }

            return doc == null ? null : _cloudinaryService.GetUrl(doc.ProviderPublicId, doc.Version, doc.ResourceType);
        }

        private sealed class PlacementAssignment
        {
            public required Term Term { get; init; }
            public Class? AssignedClass { get; init; }
        }

        private sealed class RegistrationSubmissionResult
        {
            public required CourseRegistrationResponseDto Response { get; init; }
            public required Guid StudentId { get; init; }
            public required string StudentName { get; init; }
            public required string StudentEmail { get; init; }
            public required string CourseName { get; init; }
            public required string CenterName { get; init; }
        }
    }
}
