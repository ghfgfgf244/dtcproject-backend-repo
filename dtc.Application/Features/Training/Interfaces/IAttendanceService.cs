using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using dtc.Application.Features.Training.Interfaces;
using dtc.Application.Features.Training.DTOs;

namespace dtc.Application.Features.Training.Interfaces
{
    public interface IAttendanceService
    {
        Task<bool> MarkAttendanceAsync(MarkAttendanceRequestDto request, Guid adminId);
        Task<IEnumerable<AttendanceResponseDto>> GetAttendanceByClassScheduleAsync(Guid classScheduleId);
        Task<object> GetAttendanceReportByClassAsync(Guid classId);
    }
}
