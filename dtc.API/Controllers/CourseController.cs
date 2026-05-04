using dtc.Application.Features.Training.DTOs;
using dtc.Application.Features.Training.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace dtc.API.Controllers
{
    public class CourseController : BaseApiController
    {
        private readonly ICourseService _courseService;

        public CourseController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin,TrainingManager,EnrollmentManager")]
        public async Task<IActionResult> CreateCourse([FromBody] CreateCourseRequestDto request)
        {
            var adminId = await GetInternalUserIdAsync();
            var managedCenterId = await GetManagedCenterIdAsync();
            if (managedCenterId.HasValue && request.CenterId != managedCenterId.Value)
            {
                return Fail("You can only create courses for your assigned center.");
            }

            try
            {
                var response = await _courseService.CreateCourseAsync(request, adminId);
                return Created(response, "Course created successfully.");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,TrainingManager,EnrollmentManager")]
        public async Task<IActionResult> UpdateCourse(Guid id, [FromBody] UpdateCourseRequestDto request)
        {
            var adminId = await GetInternalUserIdAsync();
            if (!await CanAccessCourseAsync(id))
            {
                return Fail("You do not have permission to access this course.");
            }

            try
            {
                var response = await _courseService.UpdateCourseAsync(id, request, adminId);
                return Ok(response, "Course updated successfully.");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,TrainingManager,EnrollmentManager")]
        public async Task<IActionResult> DeleteCourse(Guid id)
        {
            var adminId = await GetInternalUserIdAsync();
            if (!await CanAccessCourseAsync(id))
            {
                return Fail("You do not have permission to access this course.");
            }

            try
            {
                await _courseService.DeleteCourseAsync(id, adminId);
                return NoContent("Course deleted successfully.");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        [HttpPost("{id}/deactivate")]
        [Authorize(Roles = "Admin,TrainingManager,EnrollmentManager")]
        public async Task<IActionResult> DeactivateCourse(Guid id)
        {
            var adminId = await GetInternalUserIdAsync();
            if (!await CanAccessCourseAsync(id))
            {
                return Fail("You do not have permission to access this course.");
            }

            try
            {
                await _courseService.DeactivateCourseAsync(id, adminId);
                return NoContent("Course deactivated successfully.");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        [HttpPost("{id}/activate")]
        [Authorize(Roles = "Admin,TrainingManager,EnrollmentManager")]
        public async Task<IActionResult> ActivateCourse(Guid id)
        {
            var adminId = await GetInternalUserIdAsync();
            if (!await CanAccessCourseAsync(id))
            {
                return Fail("You do not have permission to access this course.");
            }

            try
            {
                await _courseService.ActivateCourseAsync(id, adminId);
                return NoContent("Course activated successfully.");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        [HttpGet("admin/all")]
        [Authorize(Roles = "Admin,TrainingManager,EnrollmentManager")]
        public async Task<IActionResult> GetAllCourses()
        {
            var response = await _courseService.GetAllCoursesAsync();
            var managedCenterId = await GetManagedCenterIdAsync();
            if (managedCenterId.HasValue)
            {
                response = response.Where(course => course.CenterId == managedCenterId.Value);
            }

            return Ok(response);
        }

        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableCourses()
        {
            var response = await _courseService.GetAvailableCoursesAsync();
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCourseDetail(Guid id)
        {
            try
            {
                var response = await _courseService.GetCourseDetailAsync(id);
                return Ok(response);
            }
            catch
            {
                return NotFound("Course");
            }
        }
    }
}
