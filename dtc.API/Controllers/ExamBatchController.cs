using dtc.Application.DTOs.Exams;
using dtc.Application.Interfaces.Exams;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace dtc.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamBatchController : ControllerBase
    {
        private readonly IExamBatchService _examBatchService;

        public ExamBatchController(IExamBatchService examBatchService)
        {
            _examBatchService = examBatchService;
        }

        // DEV-98: Create exam batch
        [HttpPost]
        [Authorize(Roles = "Admin,TrainingManager")]
        public async Task<IActionResult> CreateExamBatch([FromBody] CreateExamBatchRequestDto request)
        {
            var adminId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            try
            {
                var response = await _examBatchService.CreateExamBatchAsync(request, adminId);
                return CreatedAtAction(nameof(GetExamBatchDetail), new { id = response.Id }, response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        // DEV-101: View exam batch
        [HttpGet("{id}")]
        public async Task<IActionResult> GetExamBatchDetail(Guid id)
        {
            try
            {
                var response = await _examBatchService.GetExamBatchDetailAsync(id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return NotFound(new { Error = ex.Message });
            }
        }

        // View All
        [HttpGet]
        public async Task<IActionResult> GetAllExamBatches()
        {
            var response = await _examBatchService.GetAllExamBatchesAsync();
            return Ok(response);
        }

        // DEV-99: Edit exam batch
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,TrainingManager")]
        public async Task<IActionResult> UpdateExamBatch(Guid id, [FromBody] UpdateExamBatchRequestDto request)
        {
            var adminId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            try
            {
                var response = await _examBatchService.UpdateExamBatchAsync(id, request, adminId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        // DEV-102: Update status exam batch
        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin,TrainingManager")]
        public async Task<IActionResult> UpdateExamBatchStatus(Guid id, [FromBody] UpdateExamBatchStatusRequestDto request)
        {
            var adminId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            try
            {
                await _examBatchService.UpdateExamBatchStatusAsync(id, request, adminId);
                return Ok(new { Message = "Exam batch status updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        // DEV-100: Delete exam batch
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,TrainingManager")]
        public async Task<IActionResult> DeleteExamBatch(Guid id)
        {
            var adminId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            try
            {
                await _examBatchService.DeleteExamBatchAsync(id, adminId);
                return Ok(new { Message = "Exam batch deleted successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}
