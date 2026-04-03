using dtc.Application.Common;
using dtc.Domain.Interfaces.Permissions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace dtc.API.Controllers
{
    /// <summary>
    /// Base controller for all API endpoints.
    /// Provides helper methods for consistent ApiResponse<T> returns.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseApiController : ControllerBase
    {
        protected IActionResult Ok<T>(T data, string? message = null)
            => base.Ok(ApiResponse<T>.Ok(data, message));

        protected IActionResult Created<T>(T data, string? message = null)
            => base.StatusCode(201, ApiResponse<T>.Created(data, message));

        protected IActionResult Fail(string error)
            => BadRequest(ApiResponse<object?>.Fail(error));

        protected IActionResult NotFound(string resource)
            => base.NotFound(ApiResponse<object?>.NotFound(resource));

        protected IActionResult NoContent(string? message = null)
            => base.NoContent();

        protected async Task<Guid> GetInternalUserIdAsync()
        {
            var userIdClaim = User.FindFirst("userid")?.Value
                ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (Guid.TryParse(userIdClaim, out var userId))
            {
                return userId;
            }

            var clerkId = User.FindFirst("clerkid")?.Value;
            if (string.IsNullOrEmpty(clerkId))
            {
                throw new UnauthorizedAccessException("Unable to resolve the current user from claims.");
            }

            var userRepository = HttpContext.RequestServices.GetRequiredService<IUserRepository>();
            var user = await userRepository.FirstOrDefaultAsync(u => u.ClerkId == clerkId);
            if (user == null)
            {
                throw new UnauthorizedAccessException("The authenticated user is not synced with the internal user store.");
            }

            return user.Id;
        }

        protected Guid? GetCurrentCenterId()
        {
            var claim = User.FindFirst("center_id")?.Value;
            if (Guid.TryParse(claim, out var centerId))
                return centerId;
            return null;
        }
    }
}
