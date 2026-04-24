using dtc.Application.Common;
using dtc.Domain.Entities.Exams;
using dtc.Domain.Interfaces;
using dtc.Domain.Interfaces.Permissions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace dtc.API.Controllers
{
    /// <summary>
    /// Base controller for all API endpoints.
    /// Provides helper methods for consistent ApiResponse<T> returns.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseApiController : ControllerBase
    {
        protected IActionResult Ok<T>(T data, string? message = null)
            => base.Ok(ApiResponse<T>.Ok(data, message));

        protected IActionResult Created<T>(T data, string? message = null)
            => base.StatusCode(201, ApiResponse<T>.Created(data, message));

        protected IActionResult Fail(string error)
            => BadRequest(ApiResponse<object?>.Fail(error));

        protected IActionResult NotFound(string resource)
            => base.NotFound(ApiResponse<object?>.NotFound(resource));

        protected IActionResult NoContent(string? message = null)
            => base.NoContent();

        protected async Task<Guid> GetInternalUserIdAsync()
        {
            var userIdClaim = User.FindFirst("userid")?.Value
                ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (Guid.TryParse(userIdClaim, out var userId))
            {
                return userId;
            }

            var clerkId = User.FindFirst("clerkid")?.Value;
            if (string.IsNullOrEmpty(clerkId))
            {
                throw new UnauthorizedAccessException("Unable to resolve the current user from claims.");
            }

            var userRepository = HttpContext.RequestServices.GetRequiredService<IUserRepository>();
            var user = await userRepository.FirstOrDefaultAsync(u => u.ClerkId == clerkId);
            if (user == null)
            {
                throw new UnauthorizedAccessException("The authenticated user is not synced with the internal user store.");
            }

            return user.Id;
        }

        protected Guid? GetCurrentCenterId()
        {
            var claim = User.FindFirst("center_id")?.Value;
            if (Guid.TryParse(claim, out var centerId))
                return centerId;
            return null;
        }

        protected bool IsAdminUser()
            => User.IsInRole("Admin");

        protected bool IsCenterScopedManager()
            => User.IsInRole("TrainingManager") || User.IsInRole("EnrollmentManager");

        protected async Task<Guid?> GetManagedCenterIdAsync()
        {
            if (!IsCenterScopedManager())
            {
                return null;
            }

            var claimCenterId = GetCurrentCenterId();
            if (claimCenterId.HasValue && claimCenterId.Value != Guid.Empty)
            {
                return claimCenterId;
            }

            var userId = await GetInternalUserIdAsync();
            var unitOfWork = HttpContext.RequestServices.GetRequiredService<IUnitOfWork>();
            var userCenter = (await unitOfWork.UserCenters.FindAsync(uc => uc.UserId == userId))
                .FirstOrDefault();

            return userCenter?.CenterId;
        }

        protected async Task<bool> CanAccessCourseAsync(Guid courseId)
        {
            if (IsAdminUser() || !IsCenterScopedManager())
            {
                return true;
            }

            var centerId = await GetManagedCenterIdAsync();
            if (!centerId.HasValue)
            {
                return false;
            }

            var unitOfWork = HttpContext.RequestServices.GetRequiredService<IUnitOfWork>();
            var course = await unitOfWork.Courses.GetByIdAsync(courseId);
            return course != null && course.CenterId == centerId.Value;
        }

        protected async Task<bool> CanAccessTermAsync(Guid termId)
        {
            if (IsAdminUser() || !IsCenterScopedManager())
            {
                return true;
            }

            var unitOfWork = HttpContext.RequestServices.GetRequiredService<IUnitOfWork>();
            var term = await unitOfWork.Terms.GetByIdAsync(termId);
            return term != null && await CanAccessCourseAsync(term.CourseId);
        }

        protected async Task<bool> CanAccessClassAsync(Guid classId)
        {
            if (IsAdminUser() || !IsCenterScopedManager())
            {
                return true;
            }

            var unitOfWork = HttpContext.RequestServices.GetRequiredService<IUnitOfWork>();
            var classEntity = await unitOfWork.Classes.GetByIdAsync(classId);
            return classEntity != null && await CanAccessTermAsync(classEntity.TermId);
        }

        protected async Task<bool> CanAccessScheduleAsync(Guid scheduleId)
        {
            if (IsAdminUser() || !IsCenterScopedManager())
            {
                return true;
            }

            var unitOfWork = HttpContext.RequestServices.GetRequiredService<IUnitOfWork>();
            var schedule = await unitOfWork.ClassSchedules.GetByIdAsync(scheduleId);
            return schedule != null && await CanAccessClassAsync(schedule.ClassId);
        }

        protected async Task<bool> CanAccessRegistrationAsync(Guid registrationId)
        {
            if (IsAdminUser() || !IsCenterScopedManager())
            {
                return true;
            }

            var unitOfWork = HttpContext.RequestServices.GetRequiredService<IUnitOfWork>();
            var registration = await unitOfWork.CourseRegistrations.GetByIdAsync(registrationId);
            return registration != null && await CanAccessCourseAsync(registration.CourseId);
        }

        protected async Task<bool> CanAccessExamAsync(Guid examId)
        {
            if (IsAdminUser() || !IsCenterScopedManager())
            {
                return true;
            }

            var unitOfWork = HttpContext.RequestServices.GetRequiredService<IUnitOfWork>();
            var exam = await unitOfWork.Exams.GetByIdAsync(examId);
            return exam != null && await CanAccessCourseAsync(exam.CourseId);
        }

        protected async Task<bool> CanManageExamAsync(Guid examId)
        {
            if (IsAdminUser())
            {
                return true;
            }

            if (!IsCenterScopedManager())
            {
                return false;
            }

            var unitOfWork = HttpContext.RequestServices.GetRequiredService<IUnitOfWork>();
            var exam = await unitOfWork.Exams.GetByIdAsync(examId);
            if (exam == null)
            {
                return false;
            }

            return await CanManageExamBatchAsync(exam.ExamBatchId);
        }

        protected async Task<bool> CanAccessExamRegistrationAsync(Guid examRegistrationId)
        {
            if (IsAdminUser() || !IsCenterScopedManager())
            {
                return true;
            }

            var unitOfWork = HttpContext.RequestServices.GetRequiredService<IUnitOfWork>();
            var examRegistration = await unitOfWork.ExamRegistrations.GetByIdAsync(examRegistrationId);
            if (examRegistration == null)
            {
                return false;
            }

            var approvedRegistration = (await unitOfWork.CourseRegistrations.FindAsync(r =>
                    r.UserId == examRegistration.StudentId &&
                    r.Status == dtc.Domain.Entities.CourseRegistrationStatus.Approved))
                .OrderByDescending(r => r.RegistrationDate)
                .FirstOrDefault();

            if (approvedRegistration?.AssignedTermId != null)
            {
                return await CanAccessTermAsync(approvedRegistration.AssignedTermId.Value);
            }

            var exams = (await unitOfWork.Exams.FindAsync(e => e.ExamBatchId == examRegistration.ExamBatchId)).ToList();
            if (exams.Count == 0)
            {
                return false;
            }

            return await CanAccessCourseAsync(exams[0].CourseId);
        }

        protected async Task<bool> CanAccessExamBatchAsync(Guid examBatchId)
        {
            if (IsAdminUser())
            {
                return true;
            }

            if (!IsCenterScopedManager())
            {
                return false;
            }

            var unitOfWork = HttpContext.RequestServices.GetRequiredService<IUnitOfWork>();
            var batch = await unitOfWork.ExamBatches.GetByIdAsync(examBatchId);
            if (batch == null)
            {
                return false;
            }

            if (batch.ScopeType == ExamBatchScopeType.National)
            {
                return true;
            }

            var centerId = await GetManagedCenterIdAsync();
            if (!centerId.HasValue)
            {
                return false;
            }

            return batch.CenterId == centerId.Value;
        }

        protected async Task<bool> CanManageExamBatchAsync(Guid examBatchId)
        {
            if (IsAdminUser())
            {
                return true;
            }

            if (!IsCenterScopedManager())
            {
                return false;
            }

            var centerId = await GetManagedCenterIdAsync();
            if (!centerId.HasValue)
            {
                return false;
            }

            var unitOfWork = HttpContext.RequestServices.GetRequiredService<IUnitOfWork>();
            var batch = await unitOfWork.ExamBatches.GetByIdAsync(examBatchId);
            if (batch == null || batch.ScopeType != ExamBatchScopeType.Center)
            {
                return false;
            }

            return batch.CenterId == centerId.Value;
        }

        protected async Task<bool> CanAccessUserAsync(Guid userId)
        {
            if (IsAdminUser() || !IsCenterScopedManager())
            {
                return true;
            }

            var centerId = await GetManagedCenterIdAsync();
            if (!centerId.HasValue)
            {
                return false;
            }

            var unitOfWork = HttpContext.RequestServices.GetRequiredService<IUnitOfWork>();
            return await unitOfWork.UserCenters.AnyAsync(uc => uc.UserId == userId && uc.CenterId == centerId.Value);
        }
    }
}
