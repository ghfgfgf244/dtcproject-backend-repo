using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using dtc.Application.DTOs.Training.Attendances;

namespace dtc.Application.Interfaces.Training
{
    public interface IAttendanceService
    {
        Task<bool> MarkAttendanceAsync(MarkAttendanceRequestDto request, Guid adminId);
        Task<IEnumerable<AttendanceResponseDto>> GetAttendanceByClassScheduleAsync(Guid classScheduleId);
        Task<object> GetAttendanceReportByClassAsync(Guid classId);
    }
}
