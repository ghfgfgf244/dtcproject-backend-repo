using dtc.Application.Features.Dashboards.Interfaces;
using dtc.Application.Features.Dashboards.DTOs;

using System.Threading.Tasks;

namespace dtc.Application.Features.Dashboards.Interfaces
{
    public interface IDashboardService
    {
        Task<FinanceDashboardResponseDto> GetFinanceDashboardAsync();
        Task<AdmissionDashboardResponseDto> GetAdmissionDashboardAsync(Guid? centerId = null);
        Task<EnrollmentOperationalDashboardDto> GetEnrollmentDashboardAsync(Guid? centerId = null);
        Task<TrainingOperationalDashboardDto> GetTrainingDashboardAsync(Guid? centerId = null);
        Task<AdminOperationalDashboardDto> GetAdminDashboardAsync();
    }
}
