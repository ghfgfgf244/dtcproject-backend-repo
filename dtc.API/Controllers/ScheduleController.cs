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
            var adminId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
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
            var adminId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
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
            var adminId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
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
        [HttpGet("Class/{classId}")]
        public async Task<IActionResult> GetSchedulesByClass(Guid classId)
        {
            var response = await _scheduleService.GetSchedulesByClassAsync(classId);
            return Ok(response);
        }

        // DEV-88: Assign/Update Location
        [HttpPatch("{id}/location")]
        [Authorize(Roles = "Admin,TrainingManager")]
        public async Task<IActionResult> AssignLocation(Guid id, [FromBody] AssignLocationRequestDto request)
        {
            var adminId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
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
    }
}
