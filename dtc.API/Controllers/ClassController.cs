using dtc.Application.Features.Training.DTOs;
using dtc.Application.Features.Training.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace dtc.API.Controllers
{
    public class ClassController : BaseApiController
    {
        private readonly IClassService _classService;

        public ClassController(IClassService classService)
        {
            _classService = classService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin,TrainingManager")]
        public async Task<IActionResult> CreateClass([FromBody] CreateClassRequestDto request)
        {
            var adminId = await GetInternalUserIdAsync();
            if (!await CanAccessTermAsync(request.TermId))
            {
                return Fail("You do not have permission to create a class for this term.");
            }

            try
            {
                var response = await _classService.CreateClassAsync(request, adminId);
                return Created(response, "Class created successfully.");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,TrainingManager")]
        public async Task<IActionResult> UpdateClass(Guid id, [FromBody] UpdateClassRequestDto request)
        {
            var adminId = await GetInternalUserIdAsync();
            if (!await CanAccessClassAsync(id))
            {
                return Fail("You do not have permission to access this class.");
            }

            try
            {
                var response = await _classService.UpdateClassAsync(id, request, adminId);
                return Ok(response, "Class updated successfully.");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllClasses()
        {
            var response = await _classService.GetAllClassesAsync();
            var managedCenterId = await GetManagedCenterIdAsync();
            if (managedCenterId.HasValue)
            {
                response = response.Where(item => item.CenterId == managedCenterId.Value);
            }

            return Ok(response);
        }

        [HttpGet("term/{termId}")]
        [Authorize(Roles = "Admin,TrainingManager")]
        public async Task<IActionResult> GetClassesByTerm(Guid termId)
        {
            if (!await CanAccessTermAsync(termId))
            {
                return Fail("You do not have permission to access this term.");
            }

            var response = await _classService.GetClassesByTermAsync(termId);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetClassDetail(Guid id)
        {
            if (!await CanAccessClassAsync(id))
            {
                return Fail("You do not have permission to access this class.");
            }

            try
            {
                var response = await _classService.GetClassDetailAsync(id);
                return Ok(response);
            }
            catch
            {
                return NotFound("Class");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,TrainingManager")]
        public async Task<IActionResult> DeleteClass(Guid id)
        {
            var adminId = await GetInternalUserIdAsync();
            if (!await CanAccessClassAsync(id))
            {
                return Fail("You do not have permission to access this class.");
            }

            try
            {
                await _classService.DeleteClassAsync(id, adminId);
                return NoContent("Class deleted successfully.");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        [HttpPost("{id}/teachers")]
        [Authorize(Roles = "Admin,TrainingManager")]
        public async Task<IActionResult> AssignTeachersToClass(Guid id, [FromBody] AssignTeachersRequestDto request)
        {
            var adminId = await GetInternalUserIdAsync();
            if (!await CanAccessClassAsync(id))
            {
                return Fail("You do not have permission to access this class.");
            }

            try
            {
                await _classService.AssignTeachersToClassAsync(id, request, adminId);
                return NoContent("Teachers assigned successfully.");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        [HttpPost("{id}/students")]
        [Authorize(Roles = "Admin,TrainingManager,EnrollmentManager")]
        public async Task<IActionResult> AssignStudentsToClass(Guid id, [FromBody] AssignStudentsRequestDto request)
        {
            var adminId = await GetInternalUserIdAsync();
            if (!await CanAccessClassAsync(id))
            {
                return Fail("You do not have permission to access this class.");
            }

            try
            {
                await _classService.AssignStudentsToClassAsync(id, request, adminId);
                return NoContent("Students assigned successfully.");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        [HttpGet("teaching")]
        [Authorize(Roles = "Instructor,Admin,TrainingManager")]
        public async Task<IActionResult> GetTeachingClasses()
        {
            var instructorId = await GetInternalUserIdAsync();
            var response = await _classService.GetClassesByInstructorAsync(instructorId);
            return Ok(response);
        }

        [HttpGet("{id}/students")]
        [Authorize(Roles = "Instructor,Admin,TrainingManager")]
        public async Task<IActionResult> GetClassStudents(Guid id)
        {
            if (!await CanAccessClassAsync(id))
            {
                return Fail("You do not have permission to access this class.");
            }

            var response = await _classService.GetClassStudentsAsync(id);
            return Ok(response);
        }

        [HttpGet("{id}/available-students")]
        [Authorize(Roles = "Admin,TrainingManager,EnrollmentManager")]
        public async Task<IActionResult> GetAvailableStudents(Guid id)
        {
            if (!await CanAccessClassAsync(id))
            {
                return Fail("You do not have permission to access this class.");
            }

            var response = await _classService.GetAvailableStudentsAsync(id);
            return Ok(response);
        }

        [HttpDelete("{id}/students/{studentId}")]
        [Authorize(Roles = "Admin,TrainingManager,EnrollmentManager")]
        public async Task<IActionResult> RemoveStudentFromClass(Guid id, Guid studentId)
        {
            var adminId = await GetInternalUserIdAsync();
            if (!await CanAccessClassAsync(id))
            {
                return Fail("You do not have permission to access this class.");
            }

            try
            {
                await _classService.RemoveStudentFromClassAsync(id, studentId, adminId);
                return NoContent("Student removed successfully.");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        [HttpPost("{id}/students/{studentId}/transfer")]
        [Authorize(Roles = "Admin,TrainingManager,EnrollmentManager")]
        public async Task<IActionResult> TransferStudent(Guid id, Guid studentId, [FromBody] TransferStudentRequestDto request)
        {
            var adminId = await GetInternalUserIdAsync();
            if (!await CanAccessClassAsync(id) || !await CanAccessClassAsync(request.TargetClassId))
            {
                return Fail("You do not have permission to transfer students between these classes.");
            }

            try
            {
                await _classService.TransferStudentAsync(id, studentId, request, adminId);
                return NoContent("Student transferred successfully.");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        [HttpPost("auto-assign")]
        [Authorize(Roles = "Admin,TrainingManager")]
        public async Task<IActionResult> AutoAssignClasses([FromBody] AutoAssignClassesRequestDto request)
        {
            var adminId = await GetInternalUserIdAsync();
            if (!await CanAccessTermAsync(request.TermId))
            {
                return Fail("You do not have permission to auto-assign classes for this term.");
            }

            try
            {
                var response = await _classService.AutoAssignClassesAsync(request, adminId);
                return Ok(response, response.Message);
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        [HttpPost("auto-assign/explain")]
        [Authorize(Roles = "Admin,TrainingManager")]
        public async Task<IActionResult> PreviewAutoAssignClasses([FromBody] AutoAssignClassesRequestDto request)
        {
            if (!await CanAccessTermAsync(request.TermId))
            {
                return Fail("You do not have permission to preview auto-assignment for this term.");
            }

            try
            {
                var response = await _classService.PreviewAutoAssignClassesAsync(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }
    }
}
