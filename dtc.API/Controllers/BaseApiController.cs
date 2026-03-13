using dtc.Application.Common;
using Microsoft.AspNetCore.Mvc;

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
            => base.Ok(ApiResponse<object?>.NoContent(message));
    }
}
