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
    public class ExamController : ControllerBase
    {
        private readonly IExamService _examService;

        public ExamController(IExamService examService)
        {
            _examService = examService;
        }

        // DEV-92: Create exam
        [HttpPost]
        [Authorize(Roles = "Admin,TrainingManager")]
        public async Task<IActionResult> CreateExam([FromBody] CreateExamRequestDto request)
        {
            var adminId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            try
            {
                var response = await _examService.CreateExamAsync(request, adminId);
                return CreatedAtAction(nameof(GetExamDetail), new { id = response.Id }, response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        // DEV-93: View exam detail
        [HttpGet("{id}")]
        public async Task<IActionResult> GetExamDetail(Guid id)
        {
            try
            {
                var response = await _examService.GetExamDetailAsync(id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return NotFound(new { Error = ex.Message });
            }
        }

        // DEV-94: Update exam
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,TrainingManager")]
        public async Task<IActionResult> UpdateExam(Guid id, [FromBody] UpdateExamRequestDto request)
        {
            var adminId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            try
            {
                var response = await _examService.UpdateExamAsync(id, request, adminId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        // DEV-95: Delete exam
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,TrainingManager")]
        public async Task<IActionResult> DeleteExam(Guid id)
        {
            var adminId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            try
            {
                await _examService.DeleteExamAsync(id, adminId);
                return Ok(new { Message = "Exam deleted successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        // View All Exams
        [HttpGet]
        public async Task<IActionResult> GetAllExams()
        {
            var response = await _examService.GetAllExamsAsync();
            return Ok(response);
        }

        // DEV-96: View exam result
        [HttpGet("{id}/results")]
        public async Task<IActionResult> GetExamResults(Guid id)
        {
            try
            {
                var response = await _examService.GetExamResultsAsync(id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}
