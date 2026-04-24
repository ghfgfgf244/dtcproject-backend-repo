using dtc.Application.Features.Exams.DTOs;
using dtc.Application.Features.Exams.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dtc.API.Controllers
{
    public class ExamBatchController : BaseApiController
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
            var adminId = await GetInternalUserIdAsync();
            try
            {
                if (IsCenterScopedManager())
                {
                    var managedCenterId = await GetManagedCenterIdAsync();
                    if (!managedCenterId.HasValue && request.ScopeType == Domain.Entities.Exams.ExamBatchScopeType.Center)
                    {
                        return Fail("Your account is not assigned to any center.");
                    }

                    if (request.ScopeType == Domain.Entities.Exams.ExamBatchScopeType.Center)
                    {
                        request.CenterId = managedCenterId.Value;
                    }
                    else
                    {
                        request.CenterId = null;
                    }
                }

                var response = await _examBatchService.CreateExamBatchAsync(request, adminId);
                return Created(response, "Exam batch created successfully.");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        // DEV-101: View exam batch
        [HttpGet("{id}")]
        public async Task<IActionResult> GetExamBatchDetail(Guid id)
        {
            if (!await CanAccessExamBatchAsync(id))
            {
                return Fail("You do not have permission to access this exam batch.");
            }

            try
            {
                var response = await _examBatchService.GetExamBatchDetailAsync(id);
                return Ok(response);
            }
            catch
            {
                return NotFound("ExamBatch");
            }
        }

        // View All
        [HttpGet]
        public async Task<IActionResult> GetAllExamBatches()
        {
            var response = (await _examBatchService.GetAllExamBatchesAsync()).ToList();
            if (IsCenterScopedManager())
            {
                var filtered = new List<ExamBatchResponseDto>();
                foreach (var batch in response)
                {
                    if (await CanAccessExamBatchAsync(batch.Id))
                    {
                        filtered.Add(batch);
                    }
                }

                response = filtered;
            }

            return Ok(response);
        }

        [HttpGet("paged")]
        [Authorize(Roles = "Admin,TrainingManager")]
        public async Task<IActionResult> GetExamBatchesPaged([FromQuery] ExamBatchPagedQueryDto query)
        {
            Guid? managedCenterId = null;
            if (IsCenterScopedManager())
            {
                managedCenterId = await GetManagedCenterIdAsync();
            }

            var response = await _examBatchService.GetExamBatchesPagedAsync(query, managedCenterId);
            return Ok(response);
        }

        // DEV-99: Edit exam batch
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,TrainingManager")]
        public async Task<IActionResult> UpdateExamBatch(Guid id, [FromBody] UpdateExamBatchRequestDto request)
        {
            var adminId = await GetInternalUserIdAsync();
            if (!await CanManageExamBatchAsync(id))
            {
                return Fail("You do not have permission to update this exam batch.");
            }

            try
            {
                if (IsCenterScopedManager())
                {
                    var managedCenterId = await GetManagedCenterIdAsync();
                    if (!managedCenterId.HasValue && request.ScopeType == Domain.Entities.Exams.ExamBatchScopeType.Center)
                    {
                        return Fail("Your account is not assigned to any center.");
                    }

                    if (request.ScopeType == Domain.Entities.Exams.ExamBatchScopeType.Center)
                    {
                        request.CenterId = managedCenterId.Value;
                    }
                    else if (request.ScopeType == Domain.Entities.Exams.ExamBatchScopeType.National)
                    {
                        request.CenterId = null;
                    }
                }

                var response = await _examBatchService.UpdateExamBatchAsync(id, request, adminId);
                return Ok(response, "Exam batch updated successfully.");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        // DEV-102: Update status exam batch
        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin,TrainingManager")]
        public async Task<IActionResult> UpdateExamBatchStatus(Guid id, [FromBody] UpdateExamBatchStatusRequestDto request)
        {
            var adminId = await GetInternalUserIdAsync();
            if (!await CanManageExamBatchAsync(id))
            {
                return Fail("You do not have permission to update this exam batch.");
            }

            try
            {
                await _examBatchService.UpdateExamBatchStatusAsync(id, request, adminId);
                return NoContent("Exam batch status updated successfully.");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        // DEV-100: Delete exam batch
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,TrainingManager")]
        public async Task<IActionResult> DeleteExamBatch(Guid id)
        {
            var adminId = await GetInternalUserIdAsync();
            if (!await CanManageExamBatchAsync(id))
            {
                return Fail("You do not have permission to delete this exam batch.");
            }

            try
            {
                await _examBatchService.DeleteExamBatchAsync(id, adminId);
                return NoContent("Exam batch deleted successfully.");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }
    }
}
