using dtc.Application.Features.Training.DTOs;
using dtc.Application.Features.Training.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace dtc.API.Controllers
{
    [Authorize(Roles = "Instructor,TrainingManager,Admin")]
    public class EvaluationController : BaseApiController
    {
        private readonly IStudentEvaluationService _evaluationService;

        public EvaluationController(IStudentEvaluationService evaluationService)
        {
            _evaluationService = evaluationService;
        }

        // DEV-117: Evaluate student
        [HttpPost]
        public async Task<IActionResult> CreateEvaluation([FromBody] CreateStudentEvaluationRequestDto request)
        {
            var instructorId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            try
            {
                var response = await _evaluationService.CreateEvaluationAsync(instructorId, request);
                return Created(response, "Evaluation created successfully.");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEvaluationById(Guid id)
        {
            try
            {
                var response = await _evaluationService.GetEvaluationByIdAsync(id);
                return Ok(response);
            }
            catch
            {
                return NotFound("Evaluation");
            }
        }

        [HttpGet("student/{studentId}")]
        public async Task<IActionResult> GetEvaluationsForStudent(Guid studentId)
        {
            var response = await _evaluationService.GetEvaluationsForStudentAsync(studentId);
            return Ok(response);
        }

        [HttpGet("class/{classId}")]
        public async Task<IActionResult> GetEvaluationsByClass(Guid classId)
        {
            var response = await _evaluationService.GetEvaluationsByClassAsync(classId);
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvaluation(Guid id, [FromBody] UpdateStudentEvaluationRequestDto request)
        {
            var instructorId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            try
            {
                var response = await _evaluationService.UpdateEvaluationAsync(id, instructorId, request);
                return Ok(response, "Evaluation updated.");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvaluation(Guid id)
        {
            try
            {
                await _evaluationService.DeleteEvaluationAsync(id);
                return NoContent("Evaluation deleted successfully.");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }
    }
}
