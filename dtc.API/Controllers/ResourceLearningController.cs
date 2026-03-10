using dtc.Application.Features.Training.DTOs;
using dtc.Application.Features.Training.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace dtc.API.Controllers
{
    [Authorize(Roles = "Admin,TrainingManager,Instructor")]
    public class ResourceLearningController : BaseApiController
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
                return Created(response, "Resource learning created.");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
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
            catch
            {
                return NotFound("ResourceLearning");
            }
        }

        // DEV-124: Update resource learning
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateResourceLearning(Guid id, [FromBody] UpdateResourceLearningRequestDto request)
        {
            try
            {
                var response = await _resourceLearningService.UpdateResourceLearningAsync(id, request);
                return Ok(response, "Resource learning updated.");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        // DEV-125: Delete resource learning
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteResourceLearning(Guid id)
        {
            try
            {
                await _resourceLearningService.DeleteResourceLearningAsync(id);
                return NoContent("Resource learning deleted successfully.");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }
    }
}
