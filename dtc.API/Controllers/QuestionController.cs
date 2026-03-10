using dtc.Application.DTOs.Exams;
using dtc.Application.Interfaces.Exams;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace dtc.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,TrainingManager,Instructor")]
    public class QuestionController : ControllerBase
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
                return CreatedAtAction(nameof(GetQuestionDetail), new { id = response.Id }, response);
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
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
            catch (System.Exception ex)
            {
                return NotFound(new { Error = ex.Message });
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
                return Ok(response);
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        // DEV-114: Delete question
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            try
            {
                await _questionService.DeleteQuestionAsync(id);
                return Ok(new { Message = "Question deleted successfully." });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}
