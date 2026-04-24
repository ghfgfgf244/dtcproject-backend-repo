using dtc.Application.Features.Exams.DTOs;
using dtc.Application.Features.Exams.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
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
        [Authorize(Roles = "Student,Admin,TrainingManager,EnrollmentManager")]
        public async Task<IActionResult> Register([FromBody] CreateExamRegistrationRequestDto request)
        {
            var userId = await GetInternalUserIdAsync();
            var isStudent = User.IsInRole("Student");

            if (isStudent && request.StudentId != userId)
                return Fail("Students can only register themselves.");

            if (!isStudent && !await CanAccessUserAsync(request.StudentId))
                return Fail("You do not have permission to register this student.");

            if (!isStudent && !await CanAccessExamBatchAsync(request.ExamBatchId))
                return Fail("You do not have permission to use this exam batch.");

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
        [Authorize(Roles = "Admin,EnrollmentManager,TrainingManager")]
        public async Task<IActionResult> UpdateRegistrationStatus(Guid id, [FromBody] UpdateExamRegistrationStatusRequestDto request)
        {
            var adminId = await GetInternalUserIdAsync();
            if (!await CanAccessExamRegistrationAsync(id))
            {
                return Fail("You do not have permission to access this exam registration.");
            }

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
        [Authorize(Roles = "Admin,EnrollmentManager,TrainingManager")]
        public async Task<IActionResult> MarkAsPaid(Guid id)
        {
            var adminId = await GetInternalUserIdAsync();
            if (!await CanAccessExamRegistrationAsync(id))
            {
                return Fail("You do not have permission to access this exam registration.");
            }

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

        [HttpPatch("{id}/payment")]
        [Authorize(Roles = "Admin,EnrollmentManager,TrainingManager")]
        public async Task<IActionResult> UpdatePaymentStatus(Guid id, [FromBody] UpdateExamRegistrationPaymentRequestDto request)
        {
            var adminId = await GetInternalUserIdAsync();
            if (!await CanAccessExamRegistrationAsync(id))
            {
                return Fail("You do not have permission to access this exam registration.");
            }

            try
            {
                await _examRegistrationService.UpdatePaymentStatusAsync(id, request.IsPaid, adminId);
                return NoContent("Registration payment status updated.");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        [HttpGet("Batch/{examBatchId}")]
        [Authorize(Roles = "Admin,TrainingManager,EnrollmentManager")]
        public async Task<IActionResult> GetByExamBatch(Guid examBatchId, [FromQuery] ExamRegistrationBatchQueryDto query)
        {
            var managedCenterId = await GetManagedCenterIdAsync();
            var response = await _examRegistrationService.GetByExamBatchAsync(examBatchId, query, managedCenterId);
            return Ok(response);
        }

        [HttpGet("Term/{termId}/candidates")]
        [Authorize(Roles = "Admin,TrainingManager,EnrollmentManager")]
        public async Task<IActionResult> GetCandidatesByTerm(Guid termId, [FromQuery] Guid examBatchId)
        {
            if (!await CanAccessTermAsync(termId) || !await CanAccessExamBatchAsync(examBatchId))
            {
                return Fail("You do not have permission to access this term or exam batch.");
            }

            try
            {
                var response = await _examRegistrationService.GetCandidatesByTermAsync(termId, examBatchId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        [HttpGet("Student/{studentId}")]
        [Authorize]
        public async Task<IActionResult> GetByStudent(Guid studentId)
        {
            var userId = await GetInternalUserIdAsync();
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userRole == "Student" && studentId != userId)
                return Fail("Students can only view their own registrations.");

            if (userRole != "Student" && !await CanAccessUserAsync(studentId))
                return Fail("You do not have permission to access this student's registrations.");

            var response = await _examRegistrationService.GetByStudentAsync(studentId);
            return Ok(response);
        }

        [HttpPost("bulk")]
        [Authorize(Roles = "Admin,EnrollmentManager,TrainingManager")]
        public async Task<IActionResult> CreateBulkRegistrations([FromBody] BulkExamRegistrationRequestDto request)
        {
            var adminId = await GetInternalUserIdAsync();
            if (!await CanAccessExamBatchAsync(request.ExamBatchId))
            {
                return Fail("You do not have permission to use this exam batch.");
            }

            try
            {
                await _examRegistrationService.CreateBulkRegistrationsAsync(request, adminId);
                return NoContent("Bulk registrations created successfully.");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }
    }
}
