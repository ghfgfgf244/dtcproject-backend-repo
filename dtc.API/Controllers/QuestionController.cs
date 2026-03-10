using dtc.Application.Features.Exams.DTOs;
using dtc.Application.Features.Exams.Interfaces;
using Microsoft.AspNetCore.Authorization;
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

        // DEV-111: Create question
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

        // DEV-112: View question
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
        public async Task<IActionResult> GetAllQuestions()
        {
            var response = await _questionService.GetAllQuestionsAsync();
            return Ok(response);
        }

        // DEV-113: Update question
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

        // DEV-114: Delete question
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
