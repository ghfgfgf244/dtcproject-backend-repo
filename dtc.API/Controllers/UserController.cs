using dtc.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using System;

namespace dtc.API.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMyProfile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized(new { Error = "Invalid token." });
            }

            try
            {
                var profile = await _userService.GetUserProfileAsync(userId);
                return Ok(profile);
            }
            catch (Exception ex)
            {
                // In production, avoid exposing full exceptions.
                return NotFound(new { Error = ex.Message });
            }
        }

        [HttpPut("me")]
        [Authorize]
        public async Task<IActionResult> UpdateMyProfile([FromBody] dtc.Application.DTOs.Users.UpdateProfileRequestDto request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized(new { Error = "Invalid token." });
            }

            try
            {
                var updatedProfile = await _userService.UpdateProfileAsync(userId, request);
                return Ok(updatedProfile);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPost("me/apply-staff")]
        [Authorize]
        public async Task<IActionResult> ApplyForStaff([FromBody] dtc.Application.DTOs.Users.ApplyStaffRequestDto request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized(new { Error = "Invalid token." });
            }

            try
            {
                // Validate role request: 3 = Instructor, 5 = Collaborator
                if (request.RoleId != (int)dtc.Domain.Entities.UserRole.Instructor && 
                    request.RoleId != (int)dtc.Domain.Entities.UserRole.Collaborator)
                {
                    return BadRequest(new { Error = "You can only apply for Instructor or Collaborator roles." });
                }

                await _userService.ApplyForStaffAsync(userId, request);
                return Ok(new { Message = "Application submitted successfully! Please wait for Admin approval." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpDelete("me")]
        [Authorize]
        public async Task<IActionResult> DeleteMyProfile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized(new { Error = "Invalid token." });
            }

            try
            {
                await _userService.DeleteMyProfileAsync(userId);
                return Ok(new { Message = "User profile has been deleted." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateUser([FromBody] dtc.Application.DTOs.Users.CreateUserRequestDto request)
        {
            var adminIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (adminIdClaim == null || !Guid.TryParse(adminIdClaim.Value, out var adminId))
            {
                return Unauthorized(new { Error = "Invalid admin token." });
            }

            try
            {
                var newUser = await _userService.CreateUserAsync(request, adminId);
                return CreatedAtAction(nameof(GetAllUsers), new { id = newUser.Id }, newUser);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPost("{id}/roles/student")]
        [Authorize] // Maybe restrict depending on requirements, but often used system-to-system or user authorized
        public async Task<IActionResult> AddStudentRole(Guid id)
        {
            // Note: In a real system, you might verify if the user calling this has permission, 
            // or if it's called internally by a payment webhook.
            try
            {
                await _userService.AddStudentRoleAsync(id);
                return Ok(new { Message = "Student role assigned successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPut("{id}/roles")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUserRoles(Guid id, [FromBody] dtc.Application.DTOs.Users.UpdateUserRolesRequestDto request)
        {
            try
            {
                await _userService.UpdateUserRolesAsync(id, request);
                return Ok(new { Message = "User roles updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        [HttpPut("{id}/toggle-status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ToggleUserStatus(Guid id)
        {
            var adminIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (adminIdClaim == null || !Guid.TryParse(adminIdClaim.Value, out var adminId))
            {
                return Unauthorized(new { Error = "Invalid admin token." });
            }

            try
            {
                await _userService.ToggleUserStatusAsync(adminId, id);
                return Ok(new { Message = "User status toggled successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}
