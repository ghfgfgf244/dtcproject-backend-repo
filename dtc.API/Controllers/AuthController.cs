using dtc.Application.Features.Auth.DTOs;
using dtc.Application.Features.Auth.Interfaces;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> Sync([FromBody] SyncUserRequestDto request)
        {
            try
            {
                var response = await _authService.SyncUserAsync(request);
                return Ok(response, "User synced successfully.");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            return NoContent("Logged out successfully.");
        }
    }
}
