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
            var instructorId = await GetInternalUserIdAsync();
            try
            {
                await _attendanceService.MarkAttendanceAsync(request, instructorId);
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

        [HttpGet("Report/Class/{classId}")]
        public async Task<IActionResult> GetAttendanceReport(Guid classId)
        {
            var response = await _attendanceService.GetAttendanceReportByClassAsync(classId);
            return Ok(response);
        }

        // View individual student's attendance list
        [HttpGet("Student/{studentId}")]
        public async Task<IActionResult> GetAttendanceByStudent(Guid studentId)
        {
            var response = await _attendanceService.GetAttendanceByStudentAsync(studentId);
            return Ok(response);
        }

        // View student's summary (Present/Absent count)
        [HttpGet("Student/{studentId}/Summary")]
        public async Task<IActionResult> GetStudentAttendanceSummary(Guid studentId, [FromQuery] Guid? classId = null)
        {
            var response = await _attendanceService.GetStudentAttendanceSummaryAsync(studentId, classId);
            return Ok(response);
        }

        [HttpGet("me")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetMyAttendance()
        {
            var studentId = await GetInternalUserIdAsync();
            var response = await _attendanceService.GetMyAttendanceReportAsync(studentId);
            return Ok(response);
        }
    }
}
