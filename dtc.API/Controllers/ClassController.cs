using dtc.Application.DTOs.Training.Classes;
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
    public class ClassController : ControllerBase
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
            var adminId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            try
            {
                var response = await _classService.CreateClassAsync(request, adminId);
                return CreatedAtAction(nameof(GetClassDetail), new { id = response.Id }, response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,TrainingManager")]
        public async Task<IActionResult> UpdateClass(Guid id, [FromBody] UpdateClassRequestDto request)
        {
            var adminId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            try
            {
                var response = await _classService.UpdateClassAsync(id, request, adminId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllClasses()
        {
            var response = await _classService.GetAllClassesAsync();
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetClassDetail(Guid id)
        {
            try
            {
                var response = await _classService.GetClassDetailAsync(id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return NotFound(new { Error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,TrainingManager")]
        public async Task<IActionResult> DeleteClass(Guid id)
        {
            var adminId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            try
            {
                await _classService.DeleteClassAsync(id, adminId);
                return Ok(new { Message = "Class deleted successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPost("{id}/teachers")]
        [Authorize(Roles = "Admin,TrainingManager")]
        public async Task<IActionResult> AssignTeachersToClass(Guid id, [FromBody] AssignTeachersRequestDto request)
        {
            var adminId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            try
            {
                await _classService.AssignTeachersToClassAsync(id, request, adminId);
                return Ok(new { Message = "Teachers assigned successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPost("{id}/students")]
        [Authorize(Roles = "Admin,TrainingManager,EnrollmentManager")]
        public async Task<IActionResult> AssignStudentsToClass(Guid id, [FromBody] AssignStudentsRequestDto request)
        {
            var adminId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            try
            {
                await _classService.AssignStudentsToClassAsync(id, request, adminId);
                return Ok(new { Message = "Students assigned successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}
