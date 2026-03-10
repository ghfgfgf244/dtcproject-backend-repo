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
    public class SampleExamController : ControllerBase
    {
        private readonly ISampleExamService _sampleExamService;

        public SampleExamController(ISampleExamService sampleExamService)
        {
            _sampleExamService = sampleExamService;
        }

        // DEV-105: Create sample exam
        [HttpPost]
        [Authorize(Roles = "Admin,TrainingManager,Instructor")]
        public async Task<IActionResult> CreateSampleExam([FromBody] CreateSampleExamRequestDto request)
        {
            try
            {
                var response = await _sampleExamService.CreateSampleExamAsync(request);
                return CreatedAtAction(nameof(GetSampleExamDetail), new { id = response.Id }, response);
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        // DEV-106: View sample exam
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSampleExamDetail(Guid id)
        {
            try
            {
                var response = await _sampleExamService.GetSampleExamDetailAsync(id);
                return Ok(response);
            }
            catch (System.Exception ex)
            {
                return NotFound(new { Error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSampleExams()
        {
            var response = await _sampleExamService.GetAllSampleExamsAsync();
            return Ok(response);
        }

        // DEV-107: Update sample exam
        [HttpPut("{id}/questions")]
        [Authorize(Roles = "Admin,TrainingManager,Instructor")]
        public async Task<IActionResult> UpdateSampleExamQuestions(Guid id, [FromBody] UpdateSampleExamQuestionsRequestDto request)
        {
            try
            {
                var response = await _sampleExamService.UpdateSampleExamQuestionsAsync(id, request);
                return Ok(response);
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        // DEV-108: Delete sample exam
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,TrainingManager,Instructor")]
        public async Task<IActionResult> DeleteSampleExam(Guid id)
        {
            try
            {
                await _sampleExamService.DeleteSampleExamAsync(id);
                return Ok(new { Message = "Sample exam deleted successfully." });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        // DEV-109: Do sample test
        [HttpPost("{id}/submit")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> SubmitSampleTest(Guid id, [FromBody] SubmitSampleTestRequestDto request)
        {
            var studentId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            try
            {
                var response = await _sampleExamService.DoSampleTestAsync(id, studentId, request);
                return Ok(response);
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        // DEV-110: View score sample test
        [HttpGet("results")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetMySampleTestResults()
        {
            var studentId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            try
            {
                var response = await _sampleExamService.GetSampleTestResultsForStudentAsync(studentId);
                return Ok(response);
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}
