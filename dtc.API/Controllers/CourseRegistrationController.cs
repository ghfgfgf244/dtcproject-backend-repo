using dtc.Application.Features.Training.DTOs;
using dtc.Application.Features.Training.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace dtc.API.Controllers
{
    public class CourseRegistrationController : BaseApiController
    {
        private readonly ICourseRegistrationService _registrationService;

        public CourseRegistrationController(ICourseRegistrationService registrationService)
        {
            _registrationService = registrationService;
        }

        [HttpPost]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> RegisterCourse([FromBody] RegisterCourseRequestDto request)
        {
            var studentId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            try
            {
                var response = await _registrationService.RegisterCourseAsync(request, studentId);
                return Created(response, "Course registration submitted successfully.");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        [HttpPut("{id}/cancel")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> CancelRegistration(Guid id, [FromBody] CancelRegistrationRequestDto request)
        {
            var studentId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
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
            var adminId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
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
            var studentId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _registrationService.GetMyRegistrationsAsync(studentId);
            return Ok(response);
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin,TrainingManager,EnrollmentManager")]
        public async Task<IActionResult> GetAllRegistrations()
        {
            var response = await _registrationService.GetAllRegistrationsAsync();
            return Ok(response);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetRegistrationDetail(Guid id)
        {
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
    }
}
