using dtc.Application.Features.Collaborators.DTOs;
using dtc.Application.Features.Collaborators.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace dtc.API.Controllers
{
    [Authorize]
    public class CollaboratorController : BaseApiController
    {
        private readonly ICollaboratorService _collaboratorService;

        public CollaboratorController(ICollaboratorService collaboratorService)
        {
            _collaboratorService = collaboratorService;
        }

        // DEV-127: View collaborator's list
        [HttpGet("list")]
        [Authorize(Roles = "Admin,TrainingManager,EnrollmentManager")]
        public async Task<IActionResult> GetCollaboratorList()
        {
            var list = await _collaboratorService.GetCollaboratorListAsync();
            return Ok(list);
        }

        // DEV-128: View personal token
        [HttpGet("token")]
        [Authorize(Roles = "Collaborator")]
        public async Task<IActionResult> GetMyReferralCode()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var code = await _collaboratorService.GetMyReferralCodeAsync(userId);
            if (code == null) return NotFound("ReferralCode");
            return Ok(code);
        }

        [HttpPost("token")]
        [Authorize(Roles = "Collaborator")]
        public async Task<IActionResult> GenerateReferralCode([FromBody] CreateReferralCodeRequestDto request)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            try
            {
                var code = await _collaboratorService.GenerateReferralCodeAsync(userId, request.Code);
                return Created(code, "Referral code generated.");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        // DEV-129: View number of user using token
        [HttpGet("token/usage")]
        [Authorize(Roles = "Collaborator")]
        public async Task<IActionResult> GetTokenUsage()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var count = await _collaboratorService.GetTokenUsageCountAsync(userId);
            return Ok(new { UsedCount = count });
        }

        // DEV-130: View commission rate
        [HttpGet("commission/rate")]
        [Authorize(Roles = "Collaborator")]
        public async Task<IActionResult> GetCommissionRate()
        {
            var rate = await _collaboratorService.GetCommissionRateAsync();
            return Ok(new { CommissionRate = rate });
        }

        // DEV-131: Calculate commission
        [HttpPost("commission/calculate")]
        [Authorize(Roles = "Collaborator")]
        public async Task<IActionResult> CalculateCommissions()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            try
            {
                var commissions = await _collaboratorService.CalculateAndGetCommissionsAsync(userId);
                return Ok(commissions);
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        [HttpGet("commission")]
        [Authorize(Roles = "Collaborator")]
        public async Task<IActionResult> GetMyCommissions()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var commissions = await _collaboratorService.GetMyCommissionsAsync(userId);
            return Ok(commissions);
        }
    }
}
