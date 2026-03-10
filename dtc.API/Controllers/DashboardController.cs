using dtc.Application.Features.Dashboards.DTOs;
using dtc.Application.Features.Dashboards.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace dtc.API.Controllers
{
    [Authorize(Roles = "Admin,TrainingManager")]
    public class DashboardController : BaseApiController
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        // DEV-137: View finance dashboard
        [HttpGet("finance")]
        public async Task<IActionResult> GetFinanceDashboard()
        {
            var result = await _dashboardService.GetFinanceDashboardAsync();
            return Ok(result);
        }

        // DEV-138: View admission
        [HttpGet("admission")]
        public async Task<IActionResult> GetAdmissionDashboard()
        {
            var result = await _dashboardService.GetAdmissionDashboardAsync();
            return Ok(result);
        }
    }
}
