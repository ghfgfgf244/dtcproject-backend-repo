using dtc.Application.Features.Training.DTOs;
using dtc.Application.Features.Training.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace dtc.API.Controllers
{
    public class AttendanceController : BaseApiController
    {
        private readonly IAttendanceService _attendanceService;

        public AttendanceController(IAttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }

        // DEV-89 + DEV-90: Attendance online / Check attendance
        [HttpPost]
        [Authorize(Roles = "Admin,TrainingManager,Instructor")]
        public async Task<IActionResult> MarkAttendance([FromBody] MarkAttendanceRequestDto request)
        {
            var adminId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            try
            {
                await _attendanceService.MarkAttendanceAsync(request, adminId);
                return NoContent("Attendance marked successfully.");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        // View attendance for a specific session
        [HttpGet("Schedule/{classScheduleId}")]
        [Authorize(Roles = "Admin,TrainingManager,Instructor")]
        public async Task<IActionResult> GetAttendanceBySchedule(Guid classScheduleId)
        {
            var response = await _attendanceService.GetAttendanceByClassScheduleAsync(classScheduleId);
            return Ok(response);
        }

        // DEV-91: View attendance report by Class
        [HttpGet("Report/Class/{classId}")]
        public async Task<IActionResult> GetAttendanceReport(Guid classId)
        {
            var response = await _attendanceService.GetAttendanceReportByClassAsync(classId);
            return Ok(response);
        }
    }
}
