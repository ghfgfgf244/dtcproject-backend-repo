using dtc.Application.Features.Training.DTOs;
using dtc.Application.Features.Training.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
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
        [Authorize(Roles = "Admin,TrainingManager")]
        public async Task<IActionResult> CreateTerm([FromBody] CreateTermRequestDto request)
        {
            var adminId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
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
        [Authorize(Roles = "Admin,TrainingManager")]
        public async Task<IActionResult> UpdateTerm(Guid id, [FromBody] UpdateTermRequestDto request)
        {
            var adminId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
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
        [Authorize(Roles = "Admin,TrainingManager")]
        public async Task<IActionResult> DeleteTerm(Guid id)
        {
            var adminId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
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
        public async Task<IActionResult> GetAllTerms()
        {
            var response = await _termService.GetAllTermsAsync();
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTermDetail(Guid id)
        {
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
