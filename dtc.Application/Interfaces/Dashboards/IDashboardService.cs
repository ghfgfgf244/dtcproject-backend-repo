using dtc.Application.DTOs.Dashboards;
using System.Threading.Tasks;

namespace dtc.Application.Interfaces.Dashboards
{
    public interface IDashboardService
    {
        Task<FinanceDashboardResponseDto> GetFinanceDashboardAsync();
        Task<AdmissionDashboardResponseDto> GetAdmissionDashboardAsync();
    }
}
