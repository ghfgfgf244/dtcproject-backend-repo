using dtc.Application.Features.Training.DTOs;
using dtc.Application.Features.Training.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace dtc.API.Controllers
{
    public class TermController : BaseApiController
    {
        private readonly ITermService _termService;

        public TermController(ITermService termService)
        {
            _termService = termService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin,TrainingManager,EnrollmentManager")]
        public async Task<IActionResult> CreateTerm([FromBody] CreateTermRequestDto request)
        {
            var adminId = await GetInternalUserIdAsync();
            if (!await CanAccessCourseAsync(request.CourseId))
            {
                return Fail("You do not have permission to create a term for this course.");
            }

            try
            {
                var response = await _termService.CreateTermAsync(request, adminId);
                return Created(response, "Term created successfully.");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,TrainingManager,EnrollmentManager")]
        public async Task<IActionResult> UpdateTerm(Guid id, [FromBody] UpdateTermRequestDto request)
        {
            var adminId = await GetInternalUserIdAsync();
            if (!await CanAccessTermAsync(id))
            {
                return Fail("You do not have permission to access this term.");
            }

            try
            {
                var response = await _termService.UpdateTermAsync(id, request, adminId);
                return Ok(response, "Term updated successfully.");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,TrainingManager,EnrollmentManager")]
        public async Task<IActionResult> DeleteTerm(Guid id)
        {
            var adminId = await GetInternalUserIdAsync();
            if (!await CanAccessTermAsync(id))
            {
                return Fail("You do not have permission to access this term.");
            }

            try
            {
                await _termService.DeleteTermAsync(id, adminId);
                return NoContent("Term deleted successfully.");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin,TrainingManager,EnrollmentManager")]
        public async Task<IActionResult> GetAllTerms()
        {
            var response = await _termService.GetAllTermsAsync();
            var managedCenterId = await GetManagedCenterIdAsync();
            if (managedCenterId.HasValue)
            {
                response = response.Where(term => term.CenterId == managedCenterId.Value);
            }

            return Ok(response);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,TrainingManager,EnrollmentManager")]
        public async Task<IActionResult> GetTermDetail(Guid id)
        {
            if (!await CanAccessTermAsync(id))
            {
                return Fail("You do not have permission to access this term.");
            }

            try
            {
                var response = await _termService.GetTermDetailAsync(id);
                return Ok(response);
            }
            catch
            {
                return NotFound("Term");
            }
        }
    }
}
