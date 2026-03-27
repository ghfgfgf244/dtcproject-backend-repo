using dtc.Application.Features.Training.DTOs;
using dtc.Application.Features.Training.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace dtc.API.Controllers
{
    public class StudentDrivingDistanceController : BaseApiController
    {
        private readonly IStudentDrivingDistanceService _distanceService;

        public StudentDrivingDistanceController(IStudentDrivingDistanceService distanceService)
        {
            _distanceService = distanceService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin,TrainingManager")]
        public async Task<IActionResult> CreateDistanceRecord([FromBody] CreateStudentDrivingDistanceRequestDto request)
        {
            try
            {
                var response = await _distanceService.CreateDistanceRecordAsync(request);
                return Created(response, "Distance record created successfully.");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDistanceRecord(Guid id)
        {
            try
            {
                var response = await _distanceService.GetDistanceRecordByIdAsync(id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("{id}/actual")]
        [Authorize(Roles = "Admin,TrainingManager,Instructor")]
        public async Task<IActionResult> RecordActualDistance(Guid id, [FromBody] UpdateStudentDrivingDistanceRequestDto request)
        {
            var adminId = await GetInternalUserIdAsync();
            try
            {
                var response = await _distanceService.RecordActualDistanceAsync(id, request, adminId);
                return Ok(response, "Actual distance recorded successfully.");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        [HttpGet("my")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetMyDrivingDistances()
        {
            var studentId = await GetInternalUserIdAsync();
            var response = await _distanceService.GetMyDrivingDistancesAsync(studentId);
            return Ok(response);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,TrainingManager")]
        public async Task<IActionResult> GetAllDrivingDistances()
        {
            var response = await _distanceService.GetAllDrivingDistancesAsync();
            return Ok(response);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,TrainingManager")]
        public async Task<IActionResult> DeleteDistanceRecord(Guid id)
        {
            try
            {
                await _distanceService.DeleteDistanceRecordAsync(id);
                return NoContent("Distance record deleted successfully.");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }
    }
}
