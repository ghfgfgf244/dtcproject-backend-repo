using dtc.Application.Features.Exams.DTOs;
using dtc.Application.Features.Exams.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace dtc.API.Controllers
{
    public class SampleExamController : BaseApiController
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
                return Created(response, "Sample exam created successfully.");
            }
            catch (System.Exception ex)
            {
                return Fail(ex.Message);
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
            catch
            {
                return NotFound("SampleExam");
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
                return Ok(response, "Sample exam questions updated.");
            }
            catch (System.Exception ex)
            {
                return Fail(ex.Message);
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
                return NoContent("Sample exam deleted successfully.");
            }
            catch (System.Exception ex)
            {
                return Fail(ex.Message);
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
                return Fail(ex.Message);
            }
        }

        // DEV-110: View score sample test
        [HttpGet("results")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetMySampleTestResults()
        {
            var studentId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _sampleExamService.GetSampleTestResultsForStudentAsync(studentId);
            return Ok(response);
        }
    }
}
