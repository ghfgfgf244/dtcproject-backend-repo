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
    public class LearningRoadmapController : ControllerBase
    {
        private readonly ILearningRoadmapService _learningRoadmapService;

        public LearningRoadmapController(ILearningRoadmapService learningRoadmapService)
        {
            _learningRoadmapService = learningRoadmapService;
        }

        // DEV-119: Add learning roadmap
        [HttpPost]
        public async Task<IActionResult> CreateLearningRoadmap([FromBody] CreateLearningRoadmapRequestDto request)
        {
            try
            {
                var response = await _learningRoadmapService.CreateLearningRoadmapAsync(request);
                return CreatedAtAction(nameof(GetLearningRoadmap), new { id = response.Id }, response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        // DEV-118: View learning route / DEV-122: View learning roadmap
        [HttpGet("course/{courseId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetLearningRoadmapsByCourse(Guid courseId)
        {
            var response = await _learningRoadmapService.GetLearningRoadmapsByCourseAsync(courseId);
            return Ok(response);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetLearningRoadmap(Guid id)
        {
            try
            {
                var response = await _learningRoadmapService.GetLearningRoadmapByIdAsync(id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return NotFound(new { Error = ex.Message });
            }
        }

        // DEV-120: Update learning roadmap
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLearningRoadmap(Guid id, [FromBody] UpdateLearningRoadmapRequestDto request)
        {
            try
            {
                var response = await _learningRoadmapService.UpdateLearningRoadmapAsync(id, request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        // DEV-121: Delete learning roadmap
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLearningRoadmap(Guid id)
        {
            try
            {
                await _learningRoadmapService.DeleteLearningRoadmapAsync(id);
                return Ok(new { Message = "Learning roadmap deleted successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}
