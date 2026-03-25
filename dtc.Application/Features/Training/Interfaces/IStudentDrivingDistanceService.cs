using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using dtc.Application.Features.Training.DTOs;

namespace dtc.Application.Features.Training.Interfaces
{
    public interface IStudentDrivingDistanceService
    {
        Task<StudentDrivingDistanceResponseDto> CreateDistanceRecordAsync(CreateStudentDrivingDistanceRequestDto request);
        Task<StudentDrivingDistanceResponseDto> RecordActualDistanceAsync(Guid id, UpdateStudentDrivingDistanceRequestDto request, Guid adminId);
        Task<bool> DeleteDistanceRecordAsync(Guid id);
        Task<StudentDrivingDistanceResponseDto> GetDistanceRecordByIdAsync(Guid id);
        Task<IEnumerable<StudentDrivingDistanceResponseDto>> GetMyDrivingDistancesAsync(Guid studentId);
        Task<IEnumerable<StudentDrivingDistanceResponseDto>> GetAllDrivingDistancesAsync();
    }
}
