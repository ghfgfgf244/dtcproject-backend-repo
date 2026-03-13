using dtc.Application.Features.Users.DTOs;
using dtc.Application.Features.Users.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace dtc.API.Controllers
{
    [Route("api/users")]
    public class UserController : BaseApiController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        private Guid GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(claim, out var id) ? id : Guid.Empty;
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMyProfile()
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty) return Fail("Invalid token.");
            try
            {
                var profile = await _userService.GetUserProfileAsync(userId);
                return Ok(profile);
            }
            catch
            {
                return NotFound("User");
            }
        }

        [HttpPut("me")]
        [Authorize]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateProfileRequestDto request)
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty) return Fail("Invalid token.");
            try
            {
                var updatedProfile = await _userService.UpdateProfileAsync(userId, request);
                return Ok(updatedProfile, "Profile updated successfully.");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        [HttpPost("me/apply-staff")]
        [Authorize]
        public async Task<IActionResult> ApplyForStaff([FromBody] ApplyStaffRequestDto request)
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty) return Fail("Invalid token.");

            if (request.RoleId != (int)dtc.Domain.Entities.UserRole.Instructor &&
                request.RoleId != (int)dtc.Domain.Entities.UserRole.Collaborator)
                return Fail("You can only apply for Instructor or Collaborator roles.");

            try
            {
                await _userService.ApplyForStaffAsync(userId, request);
                return NoContent("Application submitted! Please wait for Admin approval.");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        [HttpDelete("me")]
        [Authorize]
        public async Task<IActionResult> DeleteMyProfile()
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty) return Fail("Invalid token.");
            try
            {
                await _userService.DeleteMyProfileAsync(userId);
                return NoContent("User profile has been deleted.");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequestDto request)
        {
            var adminId = GetCurrentUserId();
            if (adminId == Guid.Empty) return Fail("Invalid admin token.");
            try
            {
                var newUser = await _userService.CreateUserAsync(request, adminId);
                return Created(newUser, "User created successfully.");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        [HttpGet("students")]
        [Authorize(Roles = "Admin,TrainingManager,Instructor")]
        public async Task<IActionResult> GetStudents()
        {
            var students = await _userService.GetUsersByRoleAsync((int)dtc.Domain.Entities.UserRole.Student);
            return Ok(students);
        }

        [HttpGet("instructors")]
        [Authorize(Roles = "Admin,TrainingManager")]
        public async Task<IActionResult> GetInstructors()
        {
            var instructors = await _userService.GetUsersByRoleAsync((int)dtc.Domain.Entities.UserRole.Instructor);
            return Ok(instructors);
        }

        [HttpPost("{id}/roles/student")]
        [Authorize]
        public async Task<IActionResult> AddStudentRole(Guid id)
        {
            try
            {
                await _userService.AddStudentRoleAsync(id);
                return NoContent("Student role assigned successfully.");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        [HttpPut("{id}/roles")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUserRoles(Guid id, [FromBody] UpdateUserRolesRequestDto request)
        {
            try
            {
                await _userService.UpdateUserRolesAsync(id, request);
                return NoContent("User roles updated successfully.");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpPut("{id}/toggle-status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ToggleUserStatus(Guid id)
        {
            var adminId = GetCurrentUserId();
            if (adminId == Guid.Empty) return Fail("Invalid admin token.");
            try
            {
                await _userService.ToggleUserStatusAsync(adminId, id);
                return NoContent("User status toggled successfully.");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }
    }
}
