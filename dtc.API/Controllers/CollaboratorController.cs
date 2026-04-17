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

        // DEV-127: View collaborator's list (Old one, kept for compatibility, but deprecated by GetAdminCollaborators)
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
            var userId = await GetInternalUserIdAsync();
            var code = await _collaboratorService.GetMyReferralCodeAsync(userId);
            if (code == null) return NotFound("ReferralCode");
            return Ok(code);
        }

        [HttpPost("token")]
        [Authorize(Roles = "Collaborator")]
        public async Task<IActionResult> GenerateReferralCode([FromBody] CreateReferralCodeRequestDto request)
        {
            var userId = await GetInternalUserIdAsync();
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
            var userId = await GetInternalUserIdAsync();
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
            var userId = await GetInternalUserIdAsync();
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
            var userId = await GetInternalUserIdAsync();
            var commissions = await _collaboratorService.GetMyCommissionsAsync(userId);
            return Ok(commissions);
        }

        // ==========================================
        // ADMIN ENDPOINTS
        // ==========================================

        [HttpGet("admin/stats")]
        [Authorize(Roles = "Admin,TrainingManager,EnrollmentManager")]
        public async Task<IActionResult> GetAdminStats()
        {
            var stats = await _collaboratorService.GetAdminStatsAsync();
            return Ok(stats);
        }

        [HttpGet("admin/collaborators")]
        [Authorize(Roles = "Admin,TrainingManager,EnrollmentManager")]
        public async Task<IActionResult> GetAdminCollaborators()
        {
            var list = await _collaboratorService.GetAdminCollaboratorsAsync();
            return Ok(list);
        }

        [HttpGet("admin/commissions")]
        [Authorize(Roles = "Admin,TrainingManager,EnrollmentManager")]
        public async Task<IActionResult> GetAdminCommissions()
        {
            var list = await _collaboratorService.GetAdminCommissionsAsync();
            return Ok(list);
        }

        [HttpPut("admin/code/{collaboratorId}/toggle")]
        [Authorize(Roles = "Admin,TrainingManager,EnrollmentManager")]
        public async Task<IActionResult> ToggleReferralCode(Guid collaboratorId)
        {
            var result = await _collaboratorService.ToggleReferralCodeAsync(collaboratorId);
            if (!result) return NotFound("Referral code not found for this collaborator.");
            return Ok("Referral code toggled successfully.");
        }

        [HttpPost("admin/commissions/{collaboratorId}/pay")]
        [Authorize(Roles = "Admin,TrainingManager,EnrollmentManager")]
        public async Task<IActionResult> PayCommissions(Guid collaboratorId)
        {
            var result = await _collaboratorService.PayCommissionAsync(collaboratorId);
            if (!result) return NotFound("No pending commissions or referral code found.");
            return Ok("Commissions paid and referral usage reset successfully.");
        }
    }
}
