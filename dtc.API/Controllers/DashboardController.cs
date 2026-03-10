using dtc.Application.Interfaces.Dashboards;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace dtc.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,TrainingManager")]
    public class DashboardController : ControllerBase
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
