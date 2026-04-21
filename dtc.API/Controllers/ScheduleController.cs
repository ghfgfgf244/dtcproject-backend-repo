using dtc.Application.Features.Training.DTOs;
using dtc.Application.Features.Training.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace dtc.API.Controllers
{
    public class ScheduleController : BaseApiController
    {
        private readonly IScheduleService _scheduleService;

        public ScheduleController(IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        // DEV-84: Create Schedule
        [HttpPost]
        [Authorize(Roles = "Admin,TrainingManager")]
        public async Task<IActionResult> CreateSchedule([FromBody] CreateClassScheduleRequestDto request)
        {
            var adminId = await GetInternalUserIdAsync();
            try
            {
                var response = await _scheduleService.CreateScheduleAsync(request, adminId);
                return Created(response, "Schedule created successfully.");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        // DEV-85: Edit Schedule
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,TrainingManager")]
        public async Task<IActionResult> UpdateSchedule(Guid id, [FromBody] UpdateClassScheduleRequestDto request)
        {
            var adminId = await GetInternalUserIdAsync();
            try
            {
                var response = await _scheduleService.UpdateScheduleAsync(id, request, adminId);
                return Ok(response, "Schedule updated successfully.");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        // DEV-86: Delete Schedule
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,TrainingManager")]
        public async Task<IActionResult> DeleteSchedule(Guid id)
        {
            var adminId = await GetInternalUserIdAsync();
            try
            {
                await _scheduleService.DeleteScheduleAsync(id, adminId);
                return NoContent("Schedule deleted successfully.");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        // DEV-87: View Schedule Detail
        [HttpGet("{id}")]
        public async Task<IActionResult> GetScheduleDetail(Guid id)
        {
            try
            {
                var response = await _scheduleService.GetScheduleDetailAsync(id);
                return Ok(response);
            }
            catch
            {
                return NotFound("Schedule");
            }
        }

        // View All Schedules under a specific Class
        [HttpGet]
        [Authorize(Roles = "Admin,TrainingManager")]
        public async Task<IActionResult> GetAllSchedules()
        {
            var response = await _scheduleService.GetAllSchedulesAsync();
            return Ok(response);
        }

        [HttpGet("term/{termId}")]
        [Authorize(Roles = "Admin,TrainingManager")]
        public async Task<IActionResult> GetSchedulesByTerm(Guid termId)
        {
            var response = await _scheduleService.GetSchedulesByTermAsync(termId);
            return Ok(response);
        }

        // View All Schedules under a specific Class
        [HttpGet("Class/{classId}")]
        public async Task<IActionResult> GetSchedulesByClass(Guid classId)
        {
            var response = await _scheduleService.GetSchedulesByClassAsync(classId);
            return Ok(response);
        }

        [HttpPost("bulk")]
        [Authorize(Roles = "Admin,TrainingManager")]
        public async Task<IActionResult> CreateBulkSchedules([FromBody] BulkCreateClassScheduleRequestDto request)
        {
            var adminId = await GetInternalUserIdAsync();
            try
            {
                var response = await _scheduleService.CreateBulkSchedulesAsync(request, adminId);
                return Ok(response, "Schedules created successfully.");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        [HttpPost("import-preview")]
        [Authorize(Roles = "Admin,TrainingManager")]
        public async Task<IActionResult> ImportSchedulePreview([FromForm] IFormFile file, [FromForm] Guid? defaultInstructorId = null)
        {
            try
            {
                var response = await _scheduleService.ImportSchedulePreviewAsync(file, defaultInstructorId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        // DEV-88: Assign/Update Location
        [HttpPatch("{id}/location")]
        [Authorize(Roles = "Admin,TrainingManager")]
        public async Task<IActionResult> AssignLocation(Guid id, [FromBody] AssignLocationRequestDto request)
        {
            var adminId = await GetInternalUserIdAsync();
            try
            {
                await _scheduleService.AssignLocationAsync(id, request, adminId);
                return NoContent("Location assigned/updated successfully.");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        [HttpPost("conflict-explain")]
        [Authorize(Roles = "Admin,TrainingManager")]
        public async Task<IActionResult> ExplainConflict([FromBody] ScheduleConflictExplainRequestDto request)
        {
            try
            {
                var response = await _scheduleService.ExplainConflictAsync(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        [HttpGet("me")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetMySchedules()
        {
            var userId = await GetInternalUserIdAsync();
            var response = await _scheduleService.GetMySchedulesAsync(userId);
            return Ok(response);
        }

        [HttpGet("teaching")]
        [Authorize(Roles = "Instructor,Admin,TrainingManager")]
        public async Task<IActionResult> GetTeachingSchedule()
        {
            var instructorId = await GetInternalUserIdAsync();
            var response = await _scheduleService.GetTeachingScheduleAsync(instructorId);
            return Ok(response);
        }
    }
}
