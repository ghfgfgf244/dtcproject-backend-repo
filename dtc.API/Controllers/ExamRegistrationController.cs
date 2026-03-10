using dtc.Application.Features.Exams.DTOs;
using dtc.Application.Features.Exams.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace dtc.API.Controllers
{
    public class ExamRegistrationController : BaseApiController
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
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            if (request.StudentId != userId)
                return Fail("Students can only register themselves.");

            try
            {
                var response = await _examRegistrationService.RegisterAsync(request);
                return Created(response, "Exam registration submitted successfully.");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
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
                return NoContent("Registration status updated successfully.");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
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
                return NoContent("Registration marked as paid.");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
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
                return Fail("Students can only view their own registrations.");

            var response = await _examRegistrationService.GetByStudentAsync(studentId);
            return Ok(response);
        }
    }
}
