using dtc.Application.Features.Auth.DTOs;
using dtc.Application.Features.Auth.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using System;

namespace dtc.API.Controllers
{
    public class AuthController : BaseApiController
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("sync")]
        [Authorize]
        public async Task<IActionResult> Sync([FromBody] SyncUserRequestDto request)
        {
            try
            {
                var clerkIdFromToken = User.FindFirstValue("sub")
                    ?? User.FindFirstValue("clerkid")
                    ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrWhiteSpace(clerkIdFromToken))
                {
                    return Fail("Không xác định được danh tính người dùng từ token.");
                }

                if (!string.Equals(request.ClerkId, clerkIdFromToken, StringComparison.Ordinal))
                {
                    return Fail("ClerkId trong request không khớp với token hiện tại.");
                }

                request.Role = ResolveRoleFromClaims();
                request.CenterId = ResolveCenterIdFromClaims();

                var response = await _authService.SyncUserAsync(request);
                return Ok(response, "User synced successfully.");
            }
            catch (Exception)
            {
                return Fail("Không thể đồng bộ tài khoản lúc này.");
            }
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            return NoContent("Logged out successfully.");
        }

        private string? ResolveRoleFromClaims()
        {
            var role = User.FindFirstValue("role");
            if (!string.IsNullOrWhiteSpace(role))
            {
                return role;
            }

            return User.FindFirstValue(ClaimTypes.Role);
        }

        private Guid? ResolveCenterIdFromClaims()
        {
            var centerIdClaim = User.FindFirstValue("center_id");
            return Guid.TryParse(centerIdClaim, out var centerId) ? centerId : null;
        }
    }
}
