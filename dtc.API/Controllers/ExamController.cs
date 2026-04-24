using dtc.Application.Features.Exams.DTOs;
using dtc.Application.Features.Exams.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace dtc.API.Controllers
{
    public class ExamController : BaseApiController
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
            var adminId = await GetInternalUserIdAsync();
            if (!await CanAccessCourseAsync(request.CourseId) || !await CanAccessExamBatchAsync(request.ExamBatchId))
            {
                return Fail("You do not have permission to create an exam for this center.");
            }

            if (!IsAdminUser() && !await CanManageExamBatchAsync(request.ExamBatchId))
            {
                return Fail("You do not have permission to manage exams in this exam batch.");
            }

            try
            {
                var response = await _examService.CreateExamAsync(request, adminId);
                return Created(response, "Exam created successfully.");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        // DEV-93: View exam detail
        [HttpGet("{id}")]
        public async Task<IActionResult> GetExamDetail(Guid id)
        {
            if (!await CanAccessExamAsync(id))
            {
                return Fail("You do not have permission to access this exam.");
            }

            try
            {
                var response = await _examService.GetExamDetailAsync(id);
                return Ok(response);
            }
            catch
            {
                return NotFound("Exam");
            }
        }

        // DEV-94: Update exam
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,TrainingManager")]
        public async Task<IActionResult> UpdateExam(Guid id, [FromBody] UpdateExamRequestDto request)
        {
            var adminId = await GetInternalUserIdAsync();
            if (!await CanManageExamAsync(id))
            {
                return Fail("You do not have permission to manage this exam.");
            }

            try
            {
                var response = await _examService.UpdateExamAsync(id, request, adminId);
                return Ok(response, "Exam updated successfully.");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        // DEV-95: Delete exam
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,TrainingManager")]
        public async Task<IActionResult> DeleteExam(Guid id)
        {
            var adminId = await GetInternalUserIdAsync();
            if (!await CanManageExamAsync(id))
            {
                return Fail("You do not have permission to manage this exam.");
            }

            try
            {
                await _examService.DeleteExamAsync(id, adminId);
                return NoContent("Exam deleted successfully.");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        // View All Exams
        [HttpGet]
        public async Task<IActionResult> GetAllExams()
        {
            var response = await _examService.GetAllExamsAsync();
            var managedCenterId = await GetManagedCenterIdAsync();
            if (managedCenterId.HasValue)
            {
                response = response.Where(item => item.CenterId == managedCenterId.Value);
            }

            return Ok(response);
        }

        // DEV-96: View exam result
        [HttpGet("{id}/results")]
        public async Task<IActionResult> GetExamResults(Guid id)
        {
            if (!await CanAccessExamAsync(id))
            {
                return Fail("You do not have permission to access this exam.");
            }

            try
            {
                var response = await _examService.GetExamResultsAsync(id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        // DEV-97: Edit exam result
        [HttpPut("{id}/results/{resultId}")]
        [Authorize(Roles = "Admin,TrainingManager,Instructor")]
        public async Task<IActionResult> UpdateExamResult(Guid id, Guid resultId, [FromBody] UpdateExamResultRequestDto request)
        {
            var adminId = await GetInternalUserIdAsync();
            if (!await CanAccessExamAsync(id))
            {
                return Fail("You do not have permission to access this exam.");
            }

            try
            {
                await _examService.UpdateExamResultAsync(resultId, request, adminId);
                return NoContent("Exam result updated successfully.");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        [HttpPost("results/bulk")]
        [Authorize(Roles = "Admin,TrainingManager")]
        public async Task<IActionResult> EnterBulkExamResults([FromBody] BulkExamResultRequestDto request)
        {
            var adminId = await GetInternalUserIdAsync();
            if (!await CanAccessExamAsync(request.ExamId))
            {
                return Fail("You do not have permission to access this exam.");
            }

            try
            {
                await _examService.EnterBulkExamResultsAsync(request, adminId);
                return NoContent("Bulk exam results entered successfully.");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        [HttpGet("scoreboard")]
        [Authorize(Roles = "Admin,TrainingManager")]
        public async Task<IActionResult> GetExamScoreboard([FromQuery] ExamScoreboardQueryDto query)
        {
            if (query.CourseId.HasValue && !await CanAccessCourseAsync(query.CourseId.Value))
            {
                return Fail("You do not have permission to access this course.");
            }

            if (query.TermId.HasValue && !await CanAccessTermAsync(query.TermId.Value))
            {
                return Fail("You do not have permission to access this term.");
            }

            if (query.ExamBatchId.HasValue && !await CanAccessExamBatchAsync(query.ExamBatchId.Value))
            {
                return Fail("You do not have permission to access this exam batch.");
            }

            try
            {
                var response = await _examService.GetExamScoreboardAsync(query);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        [HttpPost("scoreboard")]
        [Authorize(Roles = "Admin,TrainingManager")]
        public async Task<IActionResult> UpsertExamScoreboard([FromBody] UpsertStudentExamScoresRequestDto request)
        {
            var adminId = await GetInternalUserIdAsync();
            if (!await CanAccessCourseAsync(request.CourseId) ||
                !await CanAccessTermAsync(request.TermId) ||
                !await CanAccessExamBatchAsync(request.ExamBatchId))
            {
                return Fail("You do not have permission to update scores for this center.");
            }

            try
            {
                var response = await _examService.UpsertStudentExamScoresAsync(request, adminId);
                return Ok(response, "Exam scores saved successfully.");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        [HttpPost("scoreboard/import")]
        [Authorize(Roles = "Admin,TrainingManager")]
        public async Task<IActionResult> ImportExamScoreboard(
            [FromForm] Guid courseId,
            [FromForm] Guid termId,
            [FromForm] Guid examBatchId,
            [FromForm] IFormFile file)
        {
            var adminId = await GetInternalUserIdAsync();
            if (!await CanAccessCourseAsync(courseId) ||
                !await CanAccessTermAsync(termId) ||
                !await CanAccessExamBatchAsync(examBatchId))
            {
                return Fail("You do not have permission to import scores for this center.");
            }

            try
            {
                var response = await _examService.ImportExamScoresAsync(
                    new ExamScoreImportRequestDto
                    {
                        CourseId = courseId,
                        TermId = termId,
                        ExamBatchId = examBatchId
                    },
                    file,
                    adminId);
                return Ok(response, "Exam scores imported successfully.");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        [HttpGet("scoreboard/import-template")]
        [Authorize(Roles = "Admin,TrainingManager")]
        public async Task<IActionResult> DownloadScoreImportTemplate(
            [FromQuery] Guid courseId,
            [FromQuery] Guid termId,
            [FromQuery] Guid examBatchId)
        {
            if (!await CanAccessCourseAsync(courseId) ||
                !await CanAccessTermAsync(termId) ||
                !await CanAccessExamBatchAsync(examBatchId))
            {
                return Fail("You do not have permission to download this template.");
            }

            try
            {
                var content = await _examService.GenerateScoreImportTemplateAsync(courseId, termId, examBatchId);
                return File(
                    fileContents: content,
                    contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileDownloadName: "exam-score-import-template.xlsx");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        [HttpGet("results/me")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetMyExamResults()
        {
            var studentId = await GetInternalUserIdAsync();
            var response = await _examService.GetMyExamResultsAsync(studentId);
            return Ok(response);
        }

        [HttpGet("me")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetMyExams()
        {
            var studentId = await GetInternalUserIdAsync();
            var response = await _examService.GetMyExamsAsync(studentId);
            return Ok(response);
        }
    }
}
