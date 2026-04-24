using dtc.Application.Features.Exams.DTOs;
using dtc.Application.Features.Exams.Interfaces;
using dtc.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace dtc.API.Controllers
{
    [Authorize(Roles = "Admin,TrainingManager,Instructor")]
    public class QuestionController : BaseApiController
    {
        private readonly IQuestionService _questionService;

        public QuestionController(IQuestionService questionService)
        {
            _questionService = questionService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateQuestion([FromBody] CreateQuestionRequestDto request)
        {
            try
            {
                var response = await _questionService.CreateQuestionAsync(request);
                return Created(response, "Question created successfully.");
            }
            catch (System.Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        [HttpPost("import")]
        public async Task<IActionResult> ImportQuestions([FromForm] IFormFile file)
        {
            try
            {
                var response = await _questionService.ImportQuestionsAsync(file);
                return Ok(response, "Question file imported successfully.");
            }
            catch (System.Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        [HttpGet("import-template")]
        public async Task<IActionResult> DownloadImportTemplate()
        {
            var content = await _questionService.GenerateImportTemplateAsync();
            return File(
                fileContents: content,
                contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileDownloadName: "question-import-template.xlsx");
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetQuestionDetail(int id)
        {
            try
            {
                var response = await _questionService.GetQuestionDetailAsync(id);
                return Ok(response);
            }
            catch
            {
                return NotFound("Question");
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllQuestions([FromQuery] string? category = null)
        {
            var response = await _questionService.GetAllQuestionsAsync(category);
            return Ok(response);
        }

        [HttpGet("common-mistakes")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCommonMistakes(
            [FromQuery] string? category = null,
            [FromQuery] ExamLevel? level = null,
            [FromQuery] int limit = 10)
        {
            var response = await _questionService.GetCommonMistakesAsync(category, level, limit);
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuestion(int id, [FromBody] UpdateQuestionRequestDto request)
        {
            try
            {
                var response = await _questionService.UpdateQuestionAsync(id, request);
                return Ok(response, "Question updated.");
            }
            catch (System.Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            try
            {
                await _questionService.DeleteQuestionAsync(id);
                return NoContent("Question deleted successfully.");
            }
            catch (System.Exception ex)
            {
                return Fail(ex.Message);
            }
        }
    }
}
