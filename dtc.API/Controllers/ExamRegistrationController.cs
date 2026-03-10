using dtc.Application.DTOs.Exams;
using dtc.Application.Interfaces.Exams;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace dtc.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamRegistrationController : ControllerBase
    {
        private readonly IExamRegistrationService _examRegistrationService;

        public ExamRegistrationController(IExamRegistrationService examRegistrationService)
        {
            _examRegistrationService = examRegistrationService;
        }

        // DEV-103: User register exam batch
        [HttpPost]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Register([FromBody] CreateExamRegistrationRequestDto request)
        {
            // Ensure student registers for themselves or Admins can do it for them (not handled here, mostly student)
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            
            if (request.StudentId != userId)
                return Forbid("Students can only register themselves.");

            try
            {
                var response = await _examRegistrationService.RegisterAsync(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        // DEV-104: Update status register exam batch
        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin,EnrollmentManager")]
        public async Task<IActionResult> UpdateRegistrationStatus(Guid id, [FromBody] UpdateExamRegistrationStatusRequestDto request)
        {
            var adminId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            try
            {
                await _examRegistrationService.UpdateStatusAsync(id, request, adminId);
                return Ok(new { Message = "Registration status updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPatch("{id}/pay")]
        [Authorize(Roles = "Admin,EnrollmentManager")]
        public async Task<IActionResult> MarkAsPaid(Guid id)
        {
            var adminId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            try
            {
                await _examRegistrationService.MarkAsPaidAsync(id, adminId);
                return Ok(new { Message = "Registration marked as paid." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpGet("Batch/{examBatchId}")]
        [Authorize(Roles = "Admin,TrainingManager,EnrollmentManager")]
        public async Task<IActionResult> GetByExamBatch(Guid examBatchId)
        {
            var response = await _examRegistrationService.GetByExamBatchAsync(examBatchId);
            return Ok(response);
        }

        [HttpGet("Student/{studentId}")]
        [Authorize]
        public async Task<IActionResult> GetByStudent(Guid studentId)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userRole == "Student" && studentId != userId)
                return Forbid("Students can only view their own registrations.");

            var response = await _examRegistrationService.GetByStudentAsync(studentId);
            return Ok(response);
        }
    }
}
