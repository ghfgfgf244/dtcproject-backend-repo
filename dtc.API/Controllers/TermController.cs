using dtc.Application.DTOs.Training.Terms;
using dtc.Application.Interfaces.Training;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace dtc.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TermController : ControllerBase
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
                return CreatedAtAction(nameof(GetTermDetail), new { id = response.Id }, response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
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
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
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
                return Ok(new { Message = "Term deleted successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
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
            catch (Exception ex)
            {
                return NotFound(new { Error = ex.Message });
            }
        }
    }
}
