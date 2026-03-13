using dtc.Application.Features.Location.DTOs;
using dtc.Application.Features.Location.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace dtc.API.Controllers
{
    public class CenterController : BaseApiController
    {
        private readonly ICenterService _centerService;

        public CenterController(ICenterService centerService)
        {
            _centerService = centerService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCenter([FromBody] CreateCenterRequestDto request)
        {
            var adminId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            try
            {
                var response = await _centerService.CreateCenterAsync(request, adminId);
                return Created(response, "Center created successfully.");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,TrainingManager")]
        public async Task<IActionResult> UpdateCenter(Guid id, [FromBody] UpdateCenterRequestDto request)
        {
            var adminId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            try
            {
                var response = await _centerService.UpdateCenterAsync(id, request, adminId);
                return Ok(response, "Center updated successfully.");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeactivateCenter(Guid id)
        {
            var adminId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            try
            {
                await _centerService.DeactivateCenterAsync(id, adminId);
                return NoContent("Center deactivated successfully.");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCenters()
        {
            var response = await _centerService.GetAllCentersAsync();
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCenterDetail(Guid id)
        {
            try
            {
                var response = await _centerService.GetCenterDetailAsync(id);
                return Ok(response);
            }
            catch
            {
                return NotFound("Center");
            }
        }

        [HttpPost("{id}/users")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignUsersToCenter(Guid id, [FromBody] AssignUsersRequestDto request)
        {
            var adminId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            try
            {
                await _centerService.AssignUsersToCenterAsync(id, request, adminId);
                return NoContent("Users assigned to center successfully.");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }
    }
}
