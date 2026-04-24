using dtc.Application.Features.Training.DTOs;
using dtc.Application.Features.Training.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
                    studentId = await GetInternalUserIdAsync();
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
        [Authorize(Roles = "Admin,TrainingManager,EnrollmentManager")]
        public async Task<IActionResult> UpdateRegistrationStatus(Guid id, [FromBody] UpdateRegistrationStatusDto request)
        {
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
        [Authorize(Roles = "Admin,TrainingManager,EnrollmentManager")]
        public async Task<IActionResult> GetAllRegistrations()
        {
            var response = await _registrationService.GetAllRegistrationsAsync();
            var managedCenterId = await GetManagedCenterIdAsync();
            if (managedCenterId.HasValue)
            {
                response = response.Where(item => item.CenterId == managedCenterId.Value);
            }

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

        [HttpGet("stats")]
        [Authorize(Roles = "Admin,TrainingManager,EnrollmentManager")]
        public async Task<IActionResult> GetRegistrationStats()
        {
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
    }
}
