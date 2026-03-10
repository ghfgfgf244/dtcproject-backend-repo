using dtc.Application.DTOs.Training;
using dtc.Application.Interfaces.Training;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace dtc.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,TrainingManager,Instructor")]
    public class ResourceLearningController : ControllerBase
    {
        private readonly IResourceLearningService _resourceLearningService;

        public ResourceLearningController(IResourceLearningService resourceLearningService)
        {
            _resourceLearningService = resourceLearningService;
        }

        // DEV-123: Add resource learning
        [HttpPost]
        public async Task<IActionResult> CreateResourceLearning([FromBody] CreateResourceLearningRequestDto request)
        {
            try
            {
                var response = await _resourceLearningService.CreateResourceLearningAsync(request);
                return CreatedAtAction(nameof(GetResourceLearning), new { id = response.Id }, response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        // DEV-126: View resource learning
        [HttpGet("course/{courseId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetResourceLearningsByCourse(Guid courseId)
        {
            var response = await _resourceLearningService.GetResourceLearningsByCourseAsync(courseId);
            return Ok(response);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetResourceLearning(Guid id)
        {
            try
            {
                var response = await _resourceLearningService.GetResourceLearningByIdAsync(id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return NotFound(new { Error = ex.Message });
            }
        }

        // DEV-124: Update resource learning
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateResourceLearning(Guid id, [FromBody] UpdateResourceLearningRequestDto request)
        {
            try
            {
                var response = await _resourceLearningService.UpdateResourceLearningAsync(id, request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        // DEV-125: Delete resource learning
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteResourceLearning(Guid id)
        {
            try
            {
                await _resourceLearningService.DeleteResourceLearningAsync(id);
                return Ok(new { Message = "Resource learning deleted successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}
