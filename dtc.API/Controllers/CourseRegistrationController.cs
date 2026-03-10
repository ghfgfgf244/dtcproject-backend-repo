using dtc.Application.DTOs.Training.Registrations;
using dtc.Application.Interfaces.Training;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace dtc.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseRegistrationController : ControllerBase
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
                return CreatedAtAction(nameof(GetRegistrationDetail), new { id = response.Id }, response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
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
                return Ok(new { Message = "Registration cancelled successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin,TrainingManager,EnrollmentManager")] // BA Feedback Integrated Here
        public async Task<IActionResult> UpdateRegistrationStatus(Guid id, [FromBody] UpdateRegistrationStatusDto request)
        {
            var adminId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            try
            {
                await _registrationService.UpdateRegistrationStatusAsync(id, request, adminId);
                return Ok(new { Message = "Status updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
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
            catch (Exception ex)
            {
                return NotFound(new { Error = ex.Message });
            }
        }
    }
}
