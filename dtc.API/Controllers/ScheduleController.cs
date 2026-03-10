using dtc.Application.DTOs.Training.Schedules;
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
    public class ScheduleController : ControllerBase
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
                return CreatedAtAction(nameof(GetScheduleDetail), new { id = response.Id }, response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
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
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
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
                return Ok(new { Message = "Schedule deleted successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
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
            catch (Exception ex)
            {
                return NotFound(new { Error = ex.Message });
            }
        }

        // View All Schedules under a specific Class
        [HttpGet("Class/{classId}")]
        public async Task<IActionResult> GetSchedulesByClass(Guid classId)
        {
            var response = await _scheduleService.GetSchedulesByClassAsync(classId);
            return Ok(response);
        }

        // DEV-88: Assign/Update Location (Address learning)
        [HttpPatch("{id}/location")]
        [Authorize(Roles = "Admin,TrainingManager")]
        public async Task<IActionResult> AssignLocation(Guid id, [FromBody] AssignLocationRequestDto request)
        {
            var adminId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            try
            {
                await _scheduleService.AssignLocationAsync(id, request, adminId);
                return Ok(new { Message = "Location assigned/updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}
