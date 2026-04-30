using dtc.Application.Features.Training.DTOs;
using dtc.Application.Features.Training.Interfaces;
using dtc.Domain.Entities;
using dtc.Domain.Interfaces.Permissions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace dtc.API.Controllers
{
    public class CourseRegistrationController : BaseApiController
    {
        private readonly ICourseRegistrationService _registrationService;

        public class RegisterCourseApiRequest
        {
            public Guid CourseId { get; set; }
            public string? FullName { get; set; }
            public string? Email { get; set; }
            public string? Phone { get; set; }
            public decimal TotalFee { get; set; }
            public string? Notes { get; set; }
            public string? ReferralCode { get; set; }
            public IFormFile? Photo { get; set; }
            public IFormFile? IdFront { get; set; }
            public IFormFile? IdBack { get; set; }
        }

        public CourseRegistrationController(ICourseRegistrationService registrationService)
        {
            _registrationService = registrationService;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterCourse([FromForm] RegisterCourseApiRequest request)
        {
            try
            {
                Guid? studentId = null;
                if (User?.Identity?.IsAuthenticated == true)
                {
                    try
                    {
                        studentId = await GetInternalUserIdAsync();
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        // Allow the registration flow to gracefully fall back to request identity data
                        // when the authenticated Clerk user has not been fully synchronized locally yet.
                        HttpContext.RequestServices
                            .GetRequiredService<Microsoft.Extensions.Logging.ILogger<CourseRegistrationController>>()
                            .LogWarning(ex, "Could not resolve internal user for authenticated course registration. Falling back to request identity data.");
                    }
                }

                var response = await _registrationService.RegisterCourseAsync(new RegisterCourseRequestDto
                {
                    CourseId = request.CourseId,
                    FullName = request.FullName,
                    Email = request.Email,
                    Phone = request.Phone,
                    TotalFee = request.TotalFee,
                    Notes = request.Notes,
                    ReferralCode = request.ReferralCode,
                    Photo = await MapFileAsync(request.Photo, "image"),
                    IdFront = await MapFileAsync(request.IdFront, "image"),
                    IdBack = await MapFileAsync(request.IdBack, "image")
                }, studentId);
                return Created(response, "Course registration submitted successfully.");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        private static async Task<UploadedFileDto?> MapFileAsync(IFormFile? file, string resourceType)
        {
            if (file == null || file.Length <= 0)
            {
                return null;
            }

            using var memoryStream = new System.IO.MemoryStream();
            await file.CopyToAsync(memoryStream);

            return new UploadedFileDto
            {
                FileName = file.FileName,
                Extension = System.IO.Path.GetExtension(file.FileName),
                ResourceType = resourceType,
                Content = memoryStream.ToArray()
            };
        }

        [HttpPut("{id}/cancel")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> CancelRegistration(Guid id, [FromBody] CancelRegistrationRequestDto request)
        {
            var studentId = await GetInternalUserIdAsync();
            try
            {
                await _registrationService.CancelRegistrationAsync(id, request.Reason, studentId);
                return NoContent("Registration cancelled successfully.");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        [HttpPut("{id}/status")]
        [Authorize]
        public async Task<IActionResult> UpdateRegistrationStatus(Guid id, [FromBody] UpdateRegistrationStatusDto request)
        {
            if (!await CanManageCourseRegistrationsAsync())
            {
                return Forbid();
            }

            var adminId = await GetInternalUserIdAsync();
            if (!await CanAccessRegistrationAsync(id))
            {
                return Fail("You do not have permission to access this registration.");
            }

            try
            {
                await _registrationService.UpdateRegistrationStatusAsync(id, request, adminId);
                return NoContent("Status updated successfully.");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        [HttpGet("me")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetMyRegistrations()
        {
            var studentId = await GetInternalUserIdAsync();
            var response = await _registrationService.GetMyRegistrationsAsync(studentId);
            return Ok(response);
        }

        [HttpGet("all")]
        [Authorize]
        public async Task<IActionResult> GetAllRegistrations()
        {
            if (!await CanManageCourseRegistrationsAsync())
            {
                return Forbid();
            }

            var response = await _registrationService.GetAllRegistrationsAsync();
            var managedCenterId = await GetManagedCenterIdAsync();
            if (managedCenterId.HasValue)
            {
                response = response.Where(item => item.CenterId == managedCenterId.Value);
            }

            return Ok(response);
        }

        [HttpGet("all/paged")]
        [Authorize]
        public async Task<IActionResult> GetRegistrationsPaged([FromQuery] CourseRegistrationPagedQueryDto query)
        {
            if (!await CanManageCourseRegistrationsAsync())
            {
                return Forbid();
            }

            var managedCenterId = await GetManagedCenterIdAsync();
            var response = await _registrationService.GetRegistrationsPagedAsync(query, managedCenterId);
            return Ok(response);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetRegistrationDetail(Guid id)
        {
            if (!await CanAccessRegistrationAsync(id))
            {
                return Fail("You do not have permission to access this registration.");
            }

            try
            {
                var response = await _registrationService.GetRegistrationDetailAsync(id);
                return Ok(response);
            }
            catch
            {
                return NotFound("CourseRegistration");
            }
        }

        [HttpGet("{id}/term-options")]
        [Authorize]
        public async Task<IActionResult> GetRegistrationTermOptions(Guid id)
        {
            if (!await CanManageCourseRegistrationsAsync())
            {
                return Forbid();
            }

            if (!await CanAccessRegistrationAsync(id))
            {
                return Fail("You do not have permission to access this registration.");
            }

            var response = await _registrationService.GetRegistrationTermOptionsAsync(id);
            return Ok(response);
        }

        [HttpPut("{id}/term-assignment")]
        [Authorize]
        public async Task<IActionResult> ReassignRegistrationTerm(Guid id, [FromBody] ReassignRegistrationTermRequestDto request)
        {
            if (!await CanManageCourseRegistrationsAsync())
            {
                return Forbid();
            }

            if (!await CanAccessRegistrationAsync(id))
            {
                return Fail("You do not have permission to access this registration.");
            }

            if (!await CanAccessTermAsync(request.TermId))
            {
                return Fail("You do not have permission to assign this registration to the selected term.");
            }

            try
            {
                var adminId = await GetInternalUserIdAsync();
                var response = await _registrationService.ReassignRegistrationTermAsync(id, request.TermId, adminId);
                return Ok(response, "Term reassigned successfully.");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        [HttpGet("stats")]
        [Authorize]
        public async Task<IActionResult> GetRegistrationStats()
        {
            if (!await CanManageCourseRegistrationsAsync())
            {
                return Forbid();
            }

            var managedCenterId = await GetManagedCenterIdAsync();
            if (managedCenterId.HasValue)
            {
                var registrations = (await _registrationService.GetAllRegistrationsAsync())
                    .Where(item => item.CenterId == managedCenterId.Value)
                    .ToList();

                return Ok(new
                {
                    NewRegistrationsThisMonth = registrations.Count(r =>
                        r.RegistrationDate >= new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1)),
                    PendingRegistrations = registrations.Count(r =>
                        string.Equals(r.Status, nameof(dtc.Domain.Entities.CourseRegistrationStatus.Pending), StringComparison.OrdinalIgnoreCase))
                });
            }

            var response = await _registrationService.GetRegistrationStatsAsync();
            return Ok(response);
        }

        private async Task<bool> CanManageCourseRegistrationsAsync()
        {
            if (HasRoleClaim("Admin") || HasRoleClaim("TrainingManager") || HasRoleClaim("EnrollmentManager"))
            {
                return true;
            }

            var userId = await GetInternalUserIdAsync();
            var userRepository = HttpContext.RequestServices.GetRequiredService<IUserRepository>();
            var user = await userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            return user.RoleId == UserRole.Admin
                || user.RoleId == UserRole.TrainingManager
                || user.RoleId == UserRole.EnrollmentManager;
        }
    }
}
